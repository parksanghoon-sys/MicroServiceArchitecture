using Application.Core.Common;
using Application.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;

namespace Application.Core
{
    public abstract class BaseDbContext<TContext> : DbContext where TContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        protected BaseDbContext(
            DbContextOptions<TContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. 현재 어셈블리의 모든 엔티티 설정 적용
            ApplyEntityConfigurations(modelBuilder);

            // 4. 서비스별 커스텀 설정
            ConfigureEntities(modelBuilder);
        }
        protected virtual void ApplyEntityConfigurations(ModelBuilder modelBuilder)
        {
            // 현재 어셈블리의 모든 IEntityTypeConfiguration 적용
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        protected virtual void ConfigureEntities(ModelBuilder modelBuilder)
        {
            // 상속받는 클래스에서 오버라이드하여 서비스별 설정 구현
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessAuditableEntities();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ProcessAuditableEntities();
            return base.SaveChanges();
        }
        private void ProcessAuditableEntities()
        {
            var currentUser = _currentUserService.GetCurrentUser();
            var utcNow = DateTime.UtcNow;

            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added ||
                           e.State == EntityState.Modified ||
                           e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.DateCreated = utcNow;
                        entry.Entity.CreatedBy = currentUser;
                        entry.Entity.DateModified = utcNow;
                        entry.Entity.ModifiedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        entry.Entity.DateModified = utcNow;
                        entry.Entity.ModifiedBy = currentUser;
                        // DateCreated와 CreatedBy는 수정하지 않음
                        entry.Property(x => x.DateCreated).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
                        break;

                    //case EntityState.Deleted:
                    //    if (entry.Entity is ISoftDelete softDeleteEntity)
                    //    {
                    //        // 소프트 딜리트 처리
                    //        entry.State = EntityState.Modified;
                    //        softDeleteEntity.IsDeleted = true;
                    //        softDeleteEntity.DateDeleted = utcNow;
                    //        softDeleteEntity.DeletedBy = currentUser;
                    //    }
                    //    break;
                }
            }
        }
    }
}
