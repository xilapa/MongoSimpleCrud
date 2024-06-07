using Microsoft.AspNetCore.Mvc;
using MongoSimpleCrud.Data;
using MongoSimpleCrud.Models;

namespace MongoSimpleCrud.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksDbContext _context;

    public BooksController(BooksDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _context.Books().GetAllAsync();
        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var book = await _context.Books().GetAsync(id);
        return Ok(book);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Book book)
    {
        await _context.Books().InsertAsync(book);
        return Ok(book);
    }

    [HttpPut("{id:guid}/update-name-using-replace")]
    public async Task<IActionResult> UpdateNameUsingReplace([FromRoute] string id, string name)
    {
        var book = await _context.Books().GetAsync(id);
        book.ChangeBookName(name);
        await _context.Books().ReplaceAsync(book);
        return Ok(book);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        await _context.Books().DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{bookId}/update-only-fields-changed")]
    public async Task Update([FromBody] Book book, [FromRoute] string bookId)
    {
        var oldBook = await _context.Books().GetAsync(bookId);
        await _context.Books().UpdateAsync(oldBook, book);
    }
    
}
