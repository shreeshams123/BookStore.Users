using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class MockDbSet<T> : DbSet<T>, IQueryable<T>, IEnumerable<T> where T : class
{
    private readonly List<T> _data;

    public MockDbSet(List<T> data)
    {
        _data = data ?? new List<T>();
        IQueryable<T> queryable = _data.AsQueryable();

        // Initialize the IQueryable properties
        this.Provider = queryable.Provider;
        this.Expression = queryable.Expression;
        this.ElementType = queryable.ElementType;
    }

    // Implemented properties from IQueryable<T>
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }
    public Type ElementType { get; }

    // Implement AddAsync method to simulate adding an entity
    public override ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _data.Add(entity);
        return new ValueTask<EntityEntry<T>>(new Mock<EntityEntry<T>>().Object);  // Return a mocked EntityEntry
    }

    // AsQueryable method to enable LINQ queries
    public override IQueryable<T> AsQueryable() => _data.AsQueryable();

    // Mocked EntityType property to avoid the CS0534 error
    public override IEntityType EntityType => null;  // Return null or a mock object

    // Implement GetEnumerator for IEnumerable<T>
    public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();

    // Implementing non-generic GetEnumerator to fulfill IEnumerable requirements
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
