using FirstASP.Data;
using FirstASP.Models;
using FirstASP.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstASP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController(FirstApiContext context, ILogger<BooksController> logger) : ControllerBase
{
    // Получить список книг. (Отфильтровать, отсортировать)
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResult<Book>>> GetBooks(
        [FromQuery] string? author = null,
        [FromQuery] int? yearFrom = null,
        [FromQuery] int? yearTo = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool ascending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageSize > 100)
            return BadRequest("PageSize cannot be more than 100");
            
        var query = context.Books.AsQueryable();
            
        // фильтрование книг
        query = ApplyFilters(query, author, yearFrom, yearTo);
        // сортировка книг
        query = ApplySorting(query, sortBy, ascending);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PaginatedResult<Book>(items, totalCount, page, pageSize));
    }

    // получить определенную книгу
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
        logger.LogInformation("Searching book with ID: {BookId}", id);
            
        var book = await context.Books.FindAsync(id);
            
        return book == null ? NotFound() : Ok(book);
    }

    // Добавить новую книгу. (Только для админов)
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Book>> AddBook(BookDto newBook)
    {
        logger.LogInformation("Addition new book");

        ValidateYear(newBook.YearPublished);

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid book data");
            return BadRequest(ModelState);
        }

        var book = new Book
        {
            Title = newBook.Title,
            Author = newBook.Author,
            YearPublished = newBook.YearPublished
        };

        context.Books.Add(book);
        await context.SaveChangesAsync();
            
        logger.LogInformation("Book with ID: {BookId}, added successfully!", book.Id);
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    // Обновить данные о книге. (Только для админов)
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
        
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateBook(int id, BookDto updatedBook)
    {
        logger.LogInformation("Update book with ID: {BookId}", id);
            
        ValidateYear(updatedBook.YearPublished);
            
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid book data");
            return BadRequest(ModelState);
        }
    
        var book = await context.Books.FindAsync(id);
        if (book == null) 
        {
            logger.LogWarning("Book with ID: {BookId} not found", id);
            return NotFound();
        }

        book.Title = updatedBook.Title;
        book.Author = updatedBook.Author;
        book.YearPublished = updatedBook.YearPublished;

        await context.SaveChangesAsync();
        return NoContent();
    }

    // Удалить определенную книгу. (Только для админов)
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        logger.LogInformation("Deleting book with ID: {Id}", id);

        var book = await context.Books.FindAsync(id);
        if (book == null) return NotFound();

        context.Books.Remove(book);
        await context.SaveChangesAsync();

        logger.LogInformation("Book with ID: {Id} deleted successfully", id);
        return NoContent();
    }

    private static IQueryable<Book> ApplyFilters(IQueryable<Book> query, 
        string? author, int? yearFrom, int? yearTo)
    {
        if (!string.IsNullOrEmpty(author))
            query = query.Where(b => b.Author.Contains(author));
    
        if (yearFrom.HasValue)
            query = query.Where(b => b.YearPublished >= yearFrom);
    
        if (yearTo.HasValue)
            query = query.Where(b => b.YearPublished <= yearTo);
    
        return query;
    }

    private static IQueryable<Book> ApplySorting(IQueryable<Book> query, 
        string? sortBy, bool ascending)
    {
        return sortBy?.ToLower() switch
        {
            "author" => ascending ? query.OrderBy(b => b.Author) 
                : query.OrderByDescending(b => b.Author),
            "year"   => ascending ? query.OrderBy(b => b.YearPublished) 
                : query.OrderByDescending(b => b.YearPublished),
            _        => ascending ? query.OrderBy(b => b.Title) 
                : query.OrderByDescending(b => b.Title)
        };
    }
    private void ValidateYear(int year)
    {
        if (year <= 0 || year > DateTime.Now.Year)
        {
            ModelState.AddModelError("YearPublished", 
                $"The year must be between 1 and {DateTime.Now.Year}");
        }
    }
}