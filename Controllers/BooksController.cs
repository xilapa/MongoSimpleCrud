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
    public async Task<IActionResult> Create(string name, string author)
    {
        var book = new Book(name, author);
        await _context.Books().InsertAsync(book);
        return Ok(book);
    }

    [HttpPut("{id:guid}/name")]
    public async Task<IActionResult> UpdateName([FromRoute] string id, string name)
    {
        var book = await _context.Books().GetAsync(id);
        book.ChangeBookName(name);
        await _context.Books().UpdateAsync(book);
        return Ok(book);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        await _context.Books().DeleteAsync(id);
        return NoContent();
    }
}
