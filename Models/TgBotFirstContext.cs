using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ttk_bot.Models;

public partial class TgBotFirstContext : DbContext
{
    public TgBotFirstContext()
    {
    }

    public TgBotFirstContext(DbContextOptions<TgBotFirstContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BranchesDim> BranchesDims { get; set; }

    public virtual DbSet<CategoriesDim> CategoriesDims { get; set; }

    public virtual DbSet<CategoriesItemsDim> CategoriesItemsDims { get; set; }

    public virtual DbSet<ContainersDim> ContainersDims { get; set; }

    public virtual DbSet<DrinksTtk> DrinksTtks { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<KbjuTtk> KbjuTtks { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    public virtual DbSet<RolesDim> RolesDims { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<SingleOrigin> SingleOrigins { get; set; }

    public virtual DbSet<SingleOriginTypesDim> SingleOriginTypesDims { get; set; }

    public virtual DbSet<SpotsDim> SpotsDims { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VolumesDim> VolumesDims { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=45.141.79.78:5433;Database=tg_bot_db;Username=postgres;Password=postgres_tg_bot");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BranchesDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("branches_dim", "statistic_prod");

            entity.Property(e => e.Branch)
                .HasColumnType("character varying")
                .HasColumnName("branch");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<CategoriesDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("categories_dim", "ttk_prod");

            entity.Property(e => e.Category)
                .HasColumnType("character varying")
                .HasColumnName("category");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<CategoriesItemsDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("categories_items_dim", "shipper_prod");

            entity.Property(e => e.Category)
                .HasColumnType("character varying")
                .HasColumnName("category");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<ContainersDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("containers_dim", "ttk_prod");

            entity.Property(e => e.Container)
                .HasColumnType("character varying")
                .HasColumnName("container");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<DrinksTtk>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("drinks_ttk", "ttk_prod");

            entity.Property(e => e.Additives)
                .HasColumnType("character varying")
                .HasColumnName("additives");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ContainerId).HasColumnName("container_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.HowToCook)
                .HasColumnType("character varying")
                .HasColumnName("how_to_cook");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ingridients)
                .HasColumnType("character varying")
                .HasColumnName("ingridients");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.PhotoPath)
                .HasColumnType("character varying")
                .HasColumnName("photo_path");
            entity.Property(e => e.Prep)
                .HasColumnType("character varying")
                .HasColumnName("prep");
            entity.Property(e => e.SpotId).HasColumnName("spot_id");
            entity.Property(e => e.VolumeId).HasColumnName("volume_id");
            entity.Property(e => e.Weight)
                .HasColumnType("character varying")
                .HasColumnName("weight");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("items", "shipper_prod");

            entity.Property(e => e.Allergens)
                .HasColumnType("character varying")
                .HasColumnName("allergens");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.Carbohydrates).HasColumnName("carbohydrates");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Composition)
                .HasColumnType("character varying")
                .HasColumnName("composition");
            entity.Property(e => e.DairyFree).HasColumnName("dairy_free");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Energy).HasColumnName("energy");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("character varying")
                .HasColumnName("expiration_date");
            entity.Property(e => e.Fats).HasColumnName("fats");
            entity.Property(e => e.GlutenFree).HasColumnName("gluten_free");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Natural100).HasColumnName("natural100");
            entity.Property(e => e.PhotoPath)
                .HasColumnType("character varying")
                .HasColumnName("photo_path");
            entity.Property(e => e.Proteins).HasColumnName("proteins");
            entity.Property(e => e.ShipperId)
                .HasColumnType("character varying")
                .HasColumnName("shipper_id");
            entity.Property(e => e.SoyaFree).HasColumnName("soya_free");
            entity.Property(e => e.StorageCond)
                .HasColumnType("character varying")
                .HasColumnName("storage_cond");
            entity.Property(e => e.SugarFree).HasColumnName("sugar_free");
            entity.Property(e => e.Vegan).HasColumnName("vegan");
            entity.Property(e => e.Weight).HasColumnName("weight");
        });

        modelBuilder.Entity<KbjuTtk>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("kbju_ttk", "ttk_prod");

            entity.Property(e => e.Caffeine).HasColumnName("caffeine");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.Carbohydrates).HasColumnName("carbohydrates");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Energy).HasColumnName("energy");
            entity.Property(e => e.Fats).HasColumnName("fats");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Proteins).HasColumnName("proteins");
            entity.Property(e => e.TtkId).HasColumnName("ttk_id");
            entity.Property(e => e.Variety)
                .HasColumnType("character varying")
                .HasColumnName("variety");
            entity.Property(e => e.VolumeId).HasColumnName("volume_id");
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("operations_pk");

            entity.ToTable("operations", "statistic_prod");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<RolesDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("roles_dim", "user_prod");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("shippers", "shipper_prod");

            entity.Property(e => e.City)
                .HasColumnType("character varying")
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasColumnType("character varying")
                .HasColumnName("country");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasColumnType("character varying")
                .HasColumnName("full_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Inn)
                .HasColumnType("character varying")
                .HasColumnName("inn");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasColumnType("character varying")
                .HasColumnName("phone_number");
            entity.Property(e => e.Region)
                .HasColumnType("character varying")
                .HasColumnName("region");
        });

        modelBuilder.Entity<SingleOrigin>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("single_origins", "single_origin_prod");

            entity.Property(e => e.Acidity)
                .HasColumnType("character varying")
                .HasColumnName("acidity");
            entity.Property(e => e.Aftertaste)
                .HasColumnType("character varying")
                .HasColumnName("aftertaste");
            entity.Property(e => e.Body)
                .HasColumnType("character varying")
                .HasColumnName("body");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Flavor)
                .HasColumnType("character varying")
                .HasColumnName("flavor");
            entity.Property(e => e.Height)
                .HasColumnType("character varying")
                .HasColumnName("height");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Process)
                .HasColumnType("character varying")
                .HasColumnName("process");
            entity.Property(e => e.Q).HasColumnName("q");
            entity.Property(e => e.Region)
                .HasColumnType("character varying")
                .HasColumnName("region");
            entity.Property(e => e.Station)
                .HasColumnType("character varying")
                .HasColumnName("station");
            entity.Property(e => e.Taste)
                .HasColumnType("character varying")
                .HasColumnName("taste");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.Variety)
                .HasColumnType("character varying")
                .HasColumnName("variety");
        });

        modelBuilder.Entity<SingleOriginTypesDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("single_origin_types_dim", "single_origin_prod");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type)
                .HasColumnType("character varying")
                .HasColumnName("type");
        });

        modelBuilder.Entity<SpotsDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("spots_dim", "user_prod");

            entity.Property(e => e.City)
                .HasColumnType("character varying")
                .HasColumnName("city");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Region)
                .HasColumnType("character varying")
                .HasColumnName("region");
            entity.Property(e => e.SpotName)
                .HasColumnType("character varying")
                .HasColumnName("spot_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("users", "user_prod");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsAccess).HasColumnName("is_access");
            entity.Property(e => e.IsAdmin).HasColumnName("is_admin");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.SpotId).HasColumnName("spot_id");
            entity.Property(e => e.TgUserId).HasColumnName("tg_user_id");
        });

        modelBuilder.Entity<VolumesDim>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("volumes_dim", "ttk_prod");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Volume)
                .HasColumnType("character varying")
                .HasColumnName("volume");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
