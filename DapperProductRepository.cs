using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Npgsql;   

namespace InventoryAppLocalDb_EF;

// DapperProductRepository.cs（Dapper 實作）
public class DapperProductRepository : IProductRepository
{
    private readonly string _connStr;

    public DapperProductRepository(string connStr)
    {
        _connStr = connStr;
    }

    // 每個方法自己開連線、用完自動關（using）
    private NpgsqlConnection CreateConn() => new NpgsqlConnection(_connStr);

    public List<Product> GetAll()
    {
        using var conn = CreateConn();
        return conn.Query<Product>(
            "SELECT * FROM products ORDER BY id"
        ).ToList();
    }

    public Product? GetById(int id)
    {
        using var conn = CreateConn();
        return conn.QueryFirstOrDefault<Product>(
            "SELECT * FROM products WHERE id = @id",
            new { id }
        );
    }

    public List<Product> GetByCategory(string category)
    {
        using var conn = CreateConn();
        return conn.Query<Product>(
            "SELECT * FROM products WHERE category = @category ORDER BY name",
            new { category }
        ).ToList();
    }

    public int Insert(Product p)
    {
        using var conn = CreateConn();
        return conn.ExecuteScalar<int>(
            @"INSERT INTO products (name, price, stock, category)
              VALUES (@Name, @Price, @Stock, @Category)
              RETURNING id",
            p
        );
    }

    public bool Update(Product p)
    {
        using var conn = CreateConn();
        int rows = conn.Execute(
            @"UPDATE products
              SET name=@Name, price=@Price, stock=@Stock, category=@Category
              WHERE id=@Id",
            p
        );
        return rows > 0;
    }

    public bool Delete(int id)
    {
        using var conn = CreateConn();
        int rows = conn.Execute(
            "DELETE FROM products WHERE id = @id",
            new { id }
        );
        return rows > 0;
    }
}