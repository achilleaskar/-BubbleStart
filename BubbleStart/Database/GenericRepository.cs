﻿using BubbleStart.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BubbleStart.Database
{
    public class GenericRepository : IDisposable
    {
        protected readonly MainDatabase Context;

        public GenericRepository()
        {
            this.Context = new MainDatabase();
            Context.Database.Log = Console.Write;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<User> FindUserAsync(string userName)
        {
            return await Context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task<Payment> Gettest()
        {
            return await Context.Payments.Where(p => p.Id == 67).FirstOrDefaultAsync();
        }

        public async Task<List<Apointment>> GetApointmentsAsync(DateTime date)
        {
            DateTime tmpEndDate = date.AddDays(6);

            return await Context.Apointments.Where(a => a.DateTime >= date && a.DateTime < tmpEndDate)
                .Include(a=>a.Customer)
                .ToListAsync();
        }

        public TEntity GetById<TEntity>(int id) where TEntity : BaseModel
        {
            return Context.Set<TEntity>().Where(p => p.Id == id).FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : BaseModel
        {
            if (filter == null)
            {
                return await Context.Set<TEntity>().ToListAsync();
            }
            else
            {
                return await Context.Set<TEntity>().Where(filter).ToListAsync();
            }
        }

        public bool HasChanges()
        {
            //var AddedEntities = Context.ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();

            //AddedEntities.ForEach(E =>
            //{
            //    if (E.CurrentValues.PropertyNames.Contains("CreatedDate"))
            //    {
            //        E.Property("CreatedDate").CurrentValue = DateTime.Now;
            //    }
            //});

            //var EditedEntities = Context.ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();

            //EditedEntities.ForEach(E =>
            //{
            //    if (E.OriginalValues.PropertyNames.Contains("ModifiedDate"))
            //    {
            //        //   E.Property("ModifiedDate").CurrentValue = DateTime.Now;
            //    }
            //});

            var changes = from e in Context.ChangeTracker.Entries()
                          where e.State.HasFlag(EntityState.Added) ||
                              e.State.HasFlag(EntityState.Modified) ||
                              e.State.HasFlag(EntityState.Deleted)
                          select e;

            foreach (var change in changes)
            {
                if (change.State == EntityState.Added)
                {
                    // Log Added
                }
                else if (change.State == EntityState.Modified)
                {
                    // Log Modified
                    var item = change.Entity;
                    var originalValues = Context.Entry(item).OriginalValues;
                    var currentValues = Context.Entry(item).CurrentValues;

                    foreach (string propertyName in originalValues.PropertyNames)
                    {
                        var original = originalValues[propertyName];
                        var current = currentValues[propertyName];

                        if (original != current)
                        {
                        }
                        Console.WriteLine("Property {0} changed from {1} to {2}",
                     propertyName,
                     originalValues[propertyName],
                     currentValues[propertyName]);
                    }
                }
                else if (change.State == EntityState.Deleted)
                {
                    // log deleted
                }
            }
            //return Context.ChangeTracker.HasChanges();
            IEnumerable<DbEntityEntry> res = from e in Context.ChangeTracker.Entries()
                                             where
                                             e.State.HasFlag(EntityState.Added) ||
                                             e.State.HasFlag(EntityState.Modified) ||
                                             e.State.HasFlag(EntityState.Deleted)
                                             select e;

            if (res.Any())
                return true;
            return false;
        }

        public async Task SaveAsync()
        {
            //var AddedEntities = Context.ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();

            //AddedEntities.ForEach(E =>
            //{
            //    if (E.CurrentValues.PropertyNames.Contains("CreatedDate"))
            //    {
            //        E.Property("CreatedDate").CurrentValue = DateTime.Now;
            //    }
            //});

            //var EditedEntities = Context.ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();

            //EditedEntities.ForEach(E =>
            //{
            //    if (E.OriginalValues.PropertyNames.Contains("ModifiedDate"))
            //    {
            //        //   E.Property("ModifiedDate").CurrentValue = DateTime.Now;
            //    }
            //});

            //var changes = from e in Context.ChangeTracker.Entries()
            //              where e.State != EntityState.Unchanged
            //              select e;

            //foreach (var change in changes)
            //{
            //    if (change.State == EntityState.Added)
            //    {
            //        // Log Added
            //    }
            //    else if (change.State == EntityState.Modified)
            //    {
            //        // Log Modified
            //        var item = change.Entity;
            //        var originalValues = Context.Entry(item).OriginalValues;
            //        var currentValues = Context.Entry(item).CurrentValues;

            //        foreach (string propertyName in originalValues.PropertyNames)
            //        {
            //            var original = originalValues[propertyName];
            //            var current = currentValues[propertyName];

            //            Console.WriteLine("Property {0} changed from {1} to {2}",
            //         propertyName,
            //         originalValues[propertyName],
            //         currentValues[propertyName]);
            //        }
            //    }
            //    else if (change.State == EntityState.Deleted)
            //    {
            //        // log deleted
            //    }
            //}
            await Context.SaveChangesAsync();
        }

        public void RollBack()
        {
            foreach (var entry in Context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;

                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
            RejectNavigationChanges();
        }

        private void RejectNavigationChanges()
        {
            var objectContext = ((IObjectContextAdapter)Context).ObjectContext;
            var deletedRelationships = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Where(e => e.IsRelationship && !this.RelationshipContainsKeyEntry(e));
            var addedRelationships = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added).Where(e => e.IsRelationship);

            foreach (var relationship in addedRelationships)
                relationship.Delete();

            foreach (var relationship in deletedRelationships)
                relationship.ChangeState(EntityState.Unchanged);
        }

        private bool RelationshipContainsKeyEntry(System.Data.Entity.Core.Objects.ObjectStateEntry stateEntry)
        {
            //prevent exception: "Cannot change state of a relationship if one of the ends of the relationship is a KeyEntry"
            //I haven't been able to find the conditions under which this happens, but it sometimes does.
            var objectContext = ((IObjectContextAdapter)Context).ObjectContext;
            var keys = new[] { stateEntry.OriginalValues[0], stateEntry.OriginalValues[1] };
            return keys.Any(key => objectContext.ObjectStateManager.GetObjectStateEntry(key).Entity == null);
        }

        internal async Task<IEnumerable<Payment>> GetAllPaymentsAsync(DateTime startDateCash, DateTime endDateCash)
        {
            try
            {
                endDateCash = endDateCash.AddDays(1);
                return await Context.Set<Payment>()
                    .Where(p => p.Date >= startDateCash && p.Date < endDateCash)
                        .Include(p => p.Customer)
                        .ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            { }
        }

        public void Add<TEntity>(TEntity model) where TEntity : BaseModel
        {
            Context.Set<TEntity>().Add(model);
        }

        public virtual void Delete<TEntity>(TEntity entity)
             where TEntity : BaseModel
        {
            var dbSet = Context.Set<TEntity>();
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsyncSortedByUserName()
        {
            return await Context.Set<User>().OrderBy(x => x.UserName).ToListAsync();
        }

        public async Task<IEnumerable<Customer>> LoadAllCustomersAsync()
        {
            try
            {
                var x = await Context.Set<Customer>()
                        .Include(c => c.Illness)
                        .Include(d => d.WeightHistory)
                        .Include(e => e.ShowUps)
                        .Include(f => f.Programs)
                        .Include(g => g.Payments)
                        .Include(h => h.Changes)
                        .ToListAsync();
                return x.OrderByDescending(c => c.ActiveCustomer).ThenBy(g => g.SureName);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            { }
        }
    }
}