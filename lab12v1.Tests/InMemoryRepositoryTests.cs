using lab12v1;
using Xunit;

namespace lab12v1.Tests;

public class InMemoryRepositoryTests
{
    private InMemoryRepository<Product> GetEmptyRepository() => new();

    [Fact]
    public void Add_ShouldAddNewItem()
    {
        var repo = GetEmptyRepository();
        var product = new Product { Id = 1, Name = "Test", Price = 10m, Stock = 5 };

        repo.Add(product);

        Assert.Single(repo.GetAll());
        Assert.Equal("Test", repo.GetById(1).Name);
    }

    [Fact]
    public void GetById_ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
    {
        var repo = GetEmptyRepository();

        Assert.Throws<KeyNotFoundException>(() => repo.GetById(99));
    }

    [Fact]
    public void GetAll_ShouldReturnAllItems()
    {
        var repo = GetEmptyRepository();
        repo.Add(new Product { Id = 1 });
        repo.Add(new Product { Id = 2 });

        var items = repo.GetAll();

        Assert.Equal(2, items.Count());
    }

    [Fact]
    public void Update_ShouldUpdateExistingItem()
    {
        var repo = GetEmptyRepository();
        repo.Add(new Product { Id = 1, Name = "Old Name" });

        repo.Update(new Product { Id = 1, Name = "New Name" });

        Assert.Equal("New Name", repo.GetById(1).Name);
    }

    [Fact]
    public void Update_ShouldThrowKeyNotFoundException_WhenItemDoesNotExist()
    {
        var repo = GetEmptyRepository();

        Assert.Throws<KeyNotFoundException>(() => repo.Update(new Product { Id = 99 }));
    }

    [Fact]
    public void Delete_ShouldRemoveItem()
    {
        var repo = GetEmptyRepository();
        repo.Add(new Product { Id = 1 });

        repo.Delete(1);

        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void Delete_ShouldThrowKeyNotFoundException_WhenItemDoesNotExist()
    {
        var repo = GetEmptyRepository();

        Assert.Throws<KeyNotFoundException>(() => repo.Delete(99));
    }
}