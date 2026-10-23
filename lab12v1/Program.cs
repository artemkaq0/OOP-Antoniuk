using lab12v1;

var repository = new InMemoryRepository<Product>();

repository.Add(new Product { Id = 1, Name = "Ноутбук", Price = 25000m, Stock = 10 });
repository.Add(new Product { Id = 2, Name = "Мишка", Price = 800m, Stock = 50 });

Console.WriteLine("Всі продукти:");
foreach (var p in repository.GetAll())
{
    Console.WriteLine(p);
}

var productToUpdate = repository.GetById(2);
productToUpdate.Price = 750m;
repository.Update(productToUpdate);

Console.WriteLine("\nПісля оновлення ціни мишки:");
Console.WriteLine(repository.GetById(2));

repository.Delete(1);
Console.WriteLine("\nПісля видалення ноутбука (залишилось):");
Console.WriteLine(repository.GetAll().Count());