using EncuestaAPI.Models;

namespace EncuestaAPI.Repositories
{
    public class Repository<T> where T : class
    {
        public EncuastasContext Context { get; }
        public Repository(EncuastasContext context)
        {
            Context = context;
        }
        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }
        //
        public T? Get(object id)
        {
            return Context.Find<T>(id);
        }
        public void Insert(T item)
        {
            Context.Add(item);
            Context.SaveChanges();
        }
        public void Update(T item)
        {
            Context.Update(item);
            Context.SaveChanges();
        }
        public void Delete(object id)
        {
            var item = Context.Find<T>(id);
            if (item != null)
            {
                Context.Remove(item);
                Context.SaveChanges();
            }
        }
    }
}
