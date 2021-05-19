using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TransnationalLanka.ThreePL.Dal.Entities;

namespace TransnationalLanka.ThreePL.Dal.Core
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ThreePlDbContext context;
        private DbSet<T> entities;

        public Repository(ThreePlDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return entities.AsQueryable();
        }

        public T GetById(long id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public void Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            context.SaveChanges();
        }

        public void Delete(long id)
        {
            T entity = entities.SingleOrDefault(s => s.Id == id);

            if (entity == null)
            {
                throw new InvalidOperationException("Unable to find entity to delete");
            }

            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
