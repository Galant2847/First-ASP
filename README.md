# Bookstore Web API  

![.NET Core](https://img.shields.io/badge/.NET-6.0-blue)  
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)  

ASP.NET Core Web API для управления книгами с JWT-аутентификацией и верификацией email.  

## 🔥 Особенности  
- **🔐 Безопасная аутентификация** через JWT-токены  
- **✉️ Верификация email** с отправкой 6-значного кода  
- **📚 Полный CRUD** для книг (доступно только админам)  
- **🔍 Умный поиск** с фильтрацией по автору/году и сортировкой  
- **📃 Пагинация** с настраиваемым размером страницы  

## 🚀 Быстрый старт  

### ⚙️ Настройка  
1. **Склонируйте репозиторий**  
   ```bash  
   git clone https://github.com/Galant2847/First-ASP.git  
   ```  

2. **Настройте конфигурацию**  
   Измените файл `appsettings.json`:  
   ```json  
   {
     "Jwt": {
       "Key": "your-strong-jwt-key-32-chars",  
       "Issuer": "https://localhost:5001",  
       "Audience": "https://localhost:5001"  
     },
     "EmailSettings": {
       "SmtpServer": "smtp.example.com",  
       "SmtpPort": 587,  
       "FromAddress": "noreply@example.com",  
       "FromName": "Bookstore API",  
       "Username": "your-email@example.com",  
       "Password": "your-email-password"  
     }
   }  
   ```  

3. **Запустите миграции**  
   ```bash  
   dotnet ef database update  
   ```  

4. **Запустите сервер**  
   ```bash  
   dotnet run  
   ```  

## 📚 Документация API  

### 🔐 Аутентификация  
| Метод | Эндпоинт | Описание |  
|-------|----------|----------|  
| `POST` | `/api/auth/register` | Регистрация нового пользователя |  
| `POST` | `/api/auth/login` | Получение JWT-токена |  
| `POST` | `/api/auth/verify-email` | Подтверждение email |  

### 📖 Управление книгами  
| Метод | Эндпоинт | Доступ | Описание |  
|-------|----------|--------|----------|  
| `GET` | `/api/books/search` | Публичный | Поиск книг |  
| `POST` | `/api/books` | Admin | Добавить книгу |  
| `PUT` | `/api/books/{id}` | Admin | Обновить книгу |  
| `DELETE` | `/api/books/{id}` | Admin | Удалить книгу |  

## 🛠 Технологический стек  
- **Backend**: ASP.NET Core 6  
- **База данных**: Entity Framework Core + SQL Server  
- **Аутентификация**: JWT Bearer  
- **Email**: SMTP через MailKit  
- **Документация**: Swagger UI  

## 🧪 Тестовые данные  
После запуска миграций автоматически создается тестовая книга:  
```json  
{
  "id": 1,
  "title": "The Great Gatsby",
  "author": "F. Scott Fitzgerald",  
  "yearPublished": 1925  
}  
```  

## 🔒 Безопасность  
Все секретные данные **не включены** в репозиторий. Для работы необходимо:  
1. Указать свои JWT-ключи в `appsettings.json`  
2. Настроить SMTP для отправки email  
