using Data.Contexts;
using Data.Interfaces;
using Domain.Extentions;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity, TModel> : IBaseRepository<TEntity, TModel> where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _table;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
        _table = context.Set<TEntity>();
    }

    public virtual async Task<RepositoryResult<bool>> AddAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult<bool> { Succeeded = false, statusCode = 400, ErrorMessage = "Entity can't be null." };

        try
        {
            _table.Add(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult<bool> { Succeeded = true, statusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool> { Succeeded = false, statusCode = 500, ErrorMessage = ex.Message };
        }
    }
    public virtual async Task<RepositoryResult<bool>> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult<bool> { Succeeded = false, statusCode = 400, ErrorMessage = "Entity can't be null." };

        try
        {
            var entityId = GetEntityId(entity);

            var tracked = _table.Local.FirstOrDefault(e => GetEntityId(e) == entityId);

            if (tracked != null)
            {
                _context.Entry(tracked).CurrentValues.SetValues(entity);
            }
            else
            {
                _table.Update(entity);
            }

            await _context.SaveChangesAsync();
            return new RepositoryResult<bool> { Succeeded = true, statusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool> { Succeeded = false, statusCode = 500, ErrorMessage = ex.Message };
        }
    }


    public virtual async Task<RepositoryResult<bool>> RemoveAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult<bool> { Succeeded = false, statusCode = 400, ErrorMessage = "Entity can't be null." };

        try
        {
            _table.Remove(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult<bool> { Succeeded = true, statusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool> { Succeeded = false, statusCode = 500, ErrorMessage = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(
        bool orderByDescending = false,
        Expression<Func<TEntity, object>>? sortBy = null,
        Expression<Func<TEntity, bool>>? where = null,
        Expression<Func<TEntity, TModel>>? select = null,
        params Expression<Func<TEntity, object>>[] includes
    )
    {
        IQueryable<TEntity> query = _table;

        if (where != null)
            query = query.Where(where);

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        if (sortBy != null)
        {
            query = orderByDescending
                ? query.OrderByDescending(sortBy)
                : query.OrderBy(sortBy);
        }

        //ChatGpt 4o
        var data = select != null
            ? await query.Select(select).ToListAsync()
            : (await query.ToListAsync()).Select(x => x.MapTo<TModel>()).ToList();
         
        //____

        return new RepositoryResult<IEnumerable<TModel>>
        { Succeeded = true, Result = data,  statusCode = 200 };
    }


    public virtual async Task<RepositoryResult<TModel?>> GetAsync(Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _table;

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        if (where == null)
            return new RepositoryResult<TModel?> { Succeeded = false, statusCode = 400, ErrorMessage = "Where can't be null." };
        var entity = await query.FirstOrDefaultAsync(where);
        if (entity == null)
            return new RepositoryResult<TModel?> { Succeeded = true, statusCode = 404, ErrorMessage = "Entity not found." };


        var result = entity.MapTo<TModel>();
        return new RepositoryResult<TModel?> { Succeeded = true, Result = result, statusCode = 200 };
    }



    public virtual async Task<RepositoryResult<bool>> ExistAsync(Expression<Func<TEntity, bool>> findBy)
    {
        var result = await _table.AnyAsync(findBy);

        if (!result)
           return new RepositoryResult<bool>
            {
                Succeeded = false,// Förfrågan lyckades tekniskt/    // Anropet lyckades tekniskt
                Result = result, // true = finns, false = finns ej /  // true om entiteten finns, false annars
                statusCode = 404, // REST: 404 om det inte finns
                ErrorMessage = "Entity not found."
            };

        return new RepositoryResult<bool> { Succeeded = true, Result = result, statusCode = 200 };
    }


    //chatGpt 4o  används för att läsa ID från ett befintligt objekt i minnet.andvänd for att updatera ett opject
    public object? GetEntityId(TEntity entity)
    {
        var prop = typeof(TEntity).GetProperty("Id");
        return prop?.GetValue(entity);
    }



}
