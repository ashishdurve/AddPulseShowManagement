using AddPulseShowManagement.Data.DBModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public class GenericRepository<T> : IEntity<T> where T : class
    {
        public MSSQLDbContext _dbContext;
        public DbSet<T> _dbSet;

        public GenericRepository(MSSQLDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = this._dbContext.Set<T>();
        }

        public T Add(T entity)
        {
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public List<T> MultipleAdd(List<T> entity)
        {
            foreach (var single in entity)
            {
                _dbSet.Add(single);
                _dbContext.SaveChanges();
            }
            return entity;
        }

        public T Edit(T entityToUpdate)
        {
            _dbSet.Add(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return entityToUpdate;
        }
        public virtual bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Any(predicate);
        }
        public List<T> Get()
        {
            return _dbSet.ToList();
        }

        public IQueryable<T> GetIQueryable()
        {
            return _dbSet;
        }

        public List<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", bool isTrackingOff = false)
        {
            var query = _dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            IQueryable<T> result = null;

            if (orderBy != null)
            {
                result = orderBy(query);
            }

            if (isTrackingOff)
                return result?.AsNoTracking().ToList();
            else
                return result?.ToList();
        }

        public List<T> Get(out int total, out int totalDisplay, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            var query = _dbSet.AsQueryable();
            total = query.Count();
            totalDisplay = total;

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            IQueryable<T> result = null;

            if (orderBy != null)
            {
                result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }

            if (isTrackingOff)
                return result?.AsNoTracking().ToList();
            else
                return result?.ToList();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public int GetCount(Expression<Func<T, bool>> filter = null)
        {
            var query = _dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }

        public void Remove(Expression<Func<T, bool>> filter)
        {
            _dbSet.RemoveRange(_dbSet.Where(filter));
        }



        public void Remove(int id)
        {
            var entityToDelete = _dbSet.Find(id);
            Remove(entityToDelete);
        }

        public void Remove(T entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
            _dbContext.SaveChanges();
        }

        public void MutipleRemove(List<T> ListEntityToDelete)
        {
            foreach (var entityToDelete in ListEntityToDelete)
            {
                if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
                {
                    _dbSet.Attach(entityToDelete);
                }
                _dbSet.Remove(entityToDelete);
                _dbContext.SaveChanges();
            }
        }
    }



    public class BaseRepository<TContext> where TContext : DbContext
    {
        protected readonly MSSQLDbContext dbContext;
        public BaseRepository(MSSQLDbContext context)
        {
            this.dbContext = context;
        }

        private T ChangeType<T>(DataRow dataRow)
        {
            var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            foreach (DataColumn col in dataRow.Table.Columns)
            {
                expando[col.ColumnName] = dataRow[col];
            }
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(expando));
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public List<T> ExecuteQuery<T>(CommandType commandType, string query, params (string, object?)[] parameters)
        {

            var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30;
            command.CommandType = commandType;

            if (parameters != null && parameters.Length > 0)
                parameters.ToList().ForEach(m => command.Parameters.AddWithValue(m.Item1, m.Item2 ?? DBNull.Value));

            var result = new DataTable();
            var data = new SqlDataAdapter(command);
            data.Fill(result);
            Console.WriteLine(Stopwatch.GetTimestamp());
            if (result.Rows.Count > 0)
                return result.Rows.OfType<DataRow>().Select(m => ChangeType<T>(m)).ToList();

            return null;
        }

        public List<T> ExecuteNonQuery<T>(CommandType commandType, string query, params (string, object?)[] parameters)
        {

            var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30;
            command.CommandType = commandType;

            if (parameters != null && parameters.Length > 0)
                parameters.ToList().ForEach(m => command.Parameters.AddWithValue(m.Item1, m.Item2 ?? DBNull.Value));

            var result = new DataTable();
            var data = new SqlDataAdapter(command);
            data.Fill(result);
            Console.WriteLine(Stopwatch.GetTimestamp());
            if (result.Rows.Count > 0)
                return result.Rows.OfType<DataRow>().Select(m => ChangeType<T>(m)).ToList();

            return null;
        }

        public List<T> ExecuteRows<T>(CommandType commandType, string query, params (string, object?)[] parameters)
        {
            var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30;
            command.CommandType = commandType;

            if (parameters != null && parameters.Length > 0)
                parameters.ToList().ForEach(m => command.Parameters.AddWithValue(m.Item1, m.Item2 ?? DBNull.Value));

            var result = new DataTable();
            var data = new SqlDataAdapter(command);
            data.Fill(result);

            if (result.Rows.Count > 0)
            {
                var r = result.Rows.OfType<DataRow>().Select(m => ChangeType<T>(m)).ToList();
                return r;
            }

            return null;
        }

        public void ExecuteNonQuery(CommandType commandType, string query, params (string, object?)[] parameters)
        {
            var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30;
            command.CommandType = commandType;

            if (parameters != null && parameters.Length > 0)
                parameters.ToList().ForEach(m => command.Parameters.AddWithValue(m.Item1, m.Item2 ?? DBNull.Value));

            var result = new DataTable();
            var data = new SqlDataAdapter(command);
            data.Fill(result);
        }

        public T ExecuteScalar<T>(CommandType commandType, string query, params (string, object?, bool)[] parameters)
        {
            var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30;
            command.CommandType = commandType;

            if (parameters != null && parameters.Length > 0)
                parameters.ToList().ForEach(m =>
                {
                    command.Parameters.AddWithValue(m.Item1, m.Item2 ?? DBNull.Value);
                    if (m.Item3 == true)
                        command.Parameters[m.Item1].Direction = ParameterDirection.Output;
                });


            var data = new SqlDataAdapter(command);
            var result = new DataTable();
            data.Fill(result);

            if (result.Rows.Count > 0 && result.Columns.Count > 0)
            {
                var amount = result.Rows[0][0];
                return (T)Convert.ChangeType(amount, typeof(T));
            }

            return default(T);
        }


        public DataSet ExecuteDataSet(CommandType commandType, string query, params (string, object?)[] parameters)
        {
            DataSet dataSet = new DataSet();

            var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30;
            command.CommandType = commandType;

            if (parameters != null && parameters.Length > 0)
                parameters.ToList().ForEach(m => command.Parameters.AddWithValue(m.Item1, m.Item2 ?? DBNull.Value));

            var result = new DataSet();
            var data = new SqlDataAdapter(command);
            data.Fill(dataSet);

            return dataSet;
        }

        public void ExecuteNonQueryWithSqlParameter(CommandType commandType, string query, params SqlParameter[] parameters)
        {
            var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 30;
            command.CommandType = commandType;

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            var result = new DataTable();
            var data = new SqlDataAdapter(command);
            data.Fill(result);
        }

        //public T GetOutputParameter<T>(CommandType commandType, string query, out Common.Response.MembershipDiscount membershipDiscount , params (string, object?, bool)[] parameters)
        //{
        //    var connection = this.dbContext.Database.GetDbConnection() as SqlConnection;
        //    membershipDiscount = new Common.Response.MembershipDiscount();
        //    using var command = connection.CreateCommand();
        //    command.CommandText = query;
        //    command.CommandTimeout = 30;
        //    command.CommandType = commandType;

        //    if (parameters != null && parameters.Length > 0)
        //        parameters.ToList().ForEach(m =>
        //        {
        //            command.Parameters.AddWithValue(m.Item1, m.Item2 ?? DBNull.Value);
        //            if (m.Item3 == true)
        //                command.Parameters[m.Item1].Direction = ParameterDirection.Output;
        //        });


        //    var data = new SqlDataAdapter(command);
        //    var result = new DataTable();
        //    data.Fill(result);

        //    if (parameters != null && parameters.Length > 0)
        //        foreach (var item in parameters)
        //        {
        //            if (item.Item3==true)
        //            {
        //                var val = Convert.ToDecimal(command.Parameters[item.Item1].Value);
        //                if (item.Item1== "@MemberShip_Fee")
        //                {
        //                    membershipDiscount.memberShip_Fee = val;
        //                }
        //                else
        //                {
        //                    membershipDiscount.memberFinalAmt = val;
        //                }
                        
        //            }
        //        }

        //    if (result.Rows.Count > 0 && result.Columns.Count > 0)
        //    {
        //        var amount = result.Rows[0][0];
        //        return (T)Convert.ChangeType(amount, typeof(T));
        //    }

        //    return default(T);
        //}
    }
}
