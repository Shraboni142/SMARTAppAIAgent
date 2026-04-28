using PagedList;
using SMART.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace SMART.Web.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext _entities;
        private readonly IDbSet<T> _dbset;
        public RepositoryBase(ApplicationDbContext context)
        {
            _entities = context;
            _dbset = context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _entities.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }
        public virtual void Update(long id, T entity)
        {
            var existingEntity = _dbset.Find(id);
            _entities.Entry(existingEntity).CurrentValues.SetValues(entity);
        }
        public virtual void Delete(T entity)
        {
            try
            {
                _dbset.Remove(entity);
            }
            catch (Exception)
            {
                AlternateDelete(entity);
            }
        }


        private string AlternateDelete(T entity)
        {
            try
            {
                if (!_entities.Set<T>().Local.Contains(entity))
                {
                    _entities.Set<T>().Attach(entity);
                }

                // Mark the entity for deletion
                _entities.Set<T>().Remove(entity);
                return "success";
            }
            catch (Exception)
            {

                return "error";
            }
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = _dbset.Where<T>(where).AsEnumerable();
            try
            {
                foreach (T obj in objects)
                    _dbset.Remove(obj);
            }
            catch (Exception)
            {
                Alternate_RemoveRange(objects);
            }
        }

        public virtual T GetById(long id)
        {
            return _dbset.Find(id);
        }
        public virtual T GetById(int id)
        {
            return _dbset.Find(id);
        }

        public virtual T GetById(string id)
        {
            return _dbset.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbset.AsNoTracking().ToList();
        }
        public virtual List<T> GetAll_List()
        {
            return _dbset.AsNoTracking().ToList();
        }
        //public virtual IEnumerable<T> GetAll()
        //{
        //    return _dbset.AsNoTracking().ToList();
        //}

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where).ToList();
            //return _dbset.Where(where).AsNoTracking().ToList();
        }

        public virtual IEnumerable<T> GetMany_With_AsNoTracking(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where).AsNoTracking().ToList();
        }
        public virtual List<T> GetMany_List(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where).AsNoTracking().ToList();
        }

        public virtual IEnumerable<T> GetMany<TOrder>(Expression<Func<T, bool>> where, int pageNumber, int pageSize, Expression<Func<T, TOrder>> orderBy, out int totalRecords)
        {
            totalRecords = _dbset.Count(where);
            return _dbset.OrderBy(orderBy).Where(where).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
        }


        /// <summary>
        /// Return a paged list of entities
        /// </summary>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="page">Which page TO retrieve</param>
        /// <param name="where">Where clause TO apply</param>
        /// <param name="order">Order by TO apply</param>
        /// <returns></returns>
        public virtual IPagedList<T> GetPage<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
        {
            var results = _dbset.OrderBy(order).Where(where).GetPage(page).AsNoTracking().ToList();
            var total = _dbset.Count(where);
            return new StaticPagedList<T>(results, page.PageNumber, page.PageSize, total);
        }

        public virtual IPagedList<T> GetPagedDescending<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> orderByDescending)
        {
            var results = _dbset.OrderByDescending(orderByDescending).Where(where).GetPage(page).AsNoTracking().ToList();
            var total = _dbset.Count(where);
            return new StaticPagedList<T>(results, page.PageNumber, page.PageSize, total);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where).FirstOrDefault<T>();
        }

        public T Get(Expression<Func<T, bool>> where, bool isUseAsNoTracking)
        {
            return _dbset.AsNoTracking().Where(where).FirstOrDefault<T>();
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return _entities.Database.SqlQuery<T>(query, parameters).ToList();
        }

        public IEnumerable<TU> GetBy<TU>(Expression<Func<T, bool>> exp, Expression<Func<T, TU>> columns)
        {
            return _dbset.Where(exp).Select(columns);
        }

        public IEnumerable<TU> GetBy<TU, TOrder>(Expression<Func<T, bool>> Where_exp, Expression<Func<T, TU>> columns, int pageNumber, int pageSize, Expression<Func<T, TOrder>> orderBy, out int totalRecords)
        {
            totalRecords = _dbset.Count(Where_exp);

            return _dbset.OrderBy(orderBy).Where(Where_exp).Select(columns).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }


        public int GetCount(Expression<Func<T, bool>> where)
        {
            return _dbset.Count(where);
        }
        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            var count = _dbset.Count(predicate);
            return count > 0;
        }

        public void AddMultiple(IEnumerable<T> list)
        {
            _entities.Set<T>().AddRange(list);
        }
        public string Add_Bulk_Insert(IEnumerable<T> list)
        {
            string responseMsg = "error";
            try
            {
                _entities.Configuration.AutoDetectChangesEnabled = false;
                _entities.Configuration.ValidateOnSaveEnabled = false;
                _entities.Set<T>().AddRange(list);
                responseMsg = "success";
                return responseMsg;
            }
            catch (Exception)
            {

                responseMsg = "Error in insert record";
                return responseMsg;
            }
            //_entities.SaveChanges();
        }

        public void DeleteMultiple(IEnumerable<T> list)
        {

            try
            {
                _entities.Set<T>().RemoveRange(list);
            }
            catch (Exception)
            {
                Alternate_RemoveRange(list);
            }

        }

        private string Alternate_RemoveRange(IEnumerable<T> list)
        {
            try
            {
                foreach (var entity in list)
                {
                    // Attach each entity to the context if not already attached
                    if (!_entities.Set<T>().Local.Contains(entity))
                    {
                        _entities.Set<T>().Attach(entity);
                    }

                    // Mark the entity for deletion
                    _entities.Set<T>().Remove(entity);
                }
                return "success";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return _entities.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }

        public IEnumerable<T> ExecStoreProcedure<T>(string sql, params object[] parameters)
        {
            return _entities.Database.SqlQuery<T>(sql, parameters);
        }
        public IEnumerable<T> SQLQueryList<T>(string sql)
        {
            return _entities.Database.SqlQuery<T>(sql);
        }

        public IEnumerable<T> SQLQueryList<T>(string sql, params object[] parameters)
        {
            return _entities.Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public T SQLQuery<T>(string sql)
        {
            return _entities.Database.SqlQuery<T>(sql).FirstOrDefault();
        }

        public T SQLQuery<T>(string sql, params SqlParameter[] parameters)
        {
            return _entities.Database.SqlQuery<T>(sql, parameters).FirstOrDefault();
        }

        public T ExecuteScalar<T>(string sqlCommand, params object[] parameters)
        {
            return _entities.Database.SqlQuery<T>(sqlCommand, parameters).FirstOrDefault();
        }

        //Added by Azad

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Any(predicate);
        }
        public T First(Expression<Func<T, bool>> predicate)
        {
            return _dbset.First(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)

        {
            return _dbset.FirstOrDefault(predicate);
        }

        public string Max(Expression<Func<T, string>> predicate)
        {
            return _dbset.AsNoTracking().Max(predicate);
        }

        public string Min(Expression<Func<T, string>> predicate)
        {
            return _dbset.Min(predicate);
        }


    }
}