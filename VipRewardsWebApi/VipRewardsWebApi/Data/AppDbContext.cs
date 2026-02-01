using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WebApiRequestLog> WebApiRequestLogs => Set<WebApiRequestLog>();
    public DbSet<XoInParam> XoInParams => Set<XoInParam>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WebApiRequestLog>(e =>
        {
            e.ToTable("WebApiRequestLog", "dbo");
            e.HasKey(x => x.Id);

            e.Property(x => x.TraceId).HasMaxLength(64);
            e.Property(x => x.RequestHeader).HasColumnType("nvarchar(max)");
            e.Property(x => x.RequestBody).HasColumnType("nvarchar(max)");
            e.Property(x => x.ResponseBody).HasColumnType("nvarchar(max)");
        });

        modelBuilder.Entity<XoInParam>(e =>
        {
            e.ToTable("XOINPARAM", "dbo");
            e.HasKey(x => new { x.SysId, x.TranId });

            e.Property(x => x.SysId).HasColumnName("SYSID").HasMaxLength(8).IsRequired();
            e.Property(x => x.TranId).HasColumnName("TRANID").HasMaxLength(32).IsRequired();
            e.Property(x => x.XoInput).HasColumnName("XO_INPUT").HasMaxLength(1024).IsRequired();
            e.Property(x => x.XoOutput).HasColumnName("XO_OUTPUT").HasMaxLength(4000).IsRequired();
            e.Property(x => x.XoFnType).HasColumnName("XO_FNTYPE").HasMaxLength(32);
            e.Property(x => x.XoClass).HasColumnName("XO_CLASS").HasMaxLength(32);
            e.Property(x => x.XoPackage).HasColumnName("XO_PACKAGE").HasMaxLength(32);
            e.Property(x => x.XoMethod).HasColumnName("XO_METHOD").HasMaxLength(32);
            e.Property(x => x.UiInput).HasColumnName("UI_INPUT").HasMaxLength(1024);
            e.Property(x => x.UiOutput).HasColumnName("UI_OUTPUT").HasMaxLength(1024);
            e.Property(x => x.UiOutputHash).HasColumnName("UI_OUTPUT_HASH").HasMaxLength(1024);
            e.Property(x => x.ChineseFieldName).HasColumnName("CHINESEFIELDNAME").HasMaxLength(1024);
        });
    }
}
