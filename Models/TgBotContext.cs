using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ttk_bot.Models;

public partial class TgBotContext : DbContext
{
    public TgBotContext()
    {
    }

    public TgBotContext(DbContextOptions<TgBotContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoriesDim> CategoriesDims { get; set; }

    public virtual DbSet<CategoriesItemsDim> CategoriesItemsDims { get; set; }

    public virtual DbSet<ContainersDim> ContainersDims { get; set; }

    public virtual DbSet<DrinksTtk> DrinksTtks { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<KbjuTtk> KbjuTtks { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<SingleOrigin> SingleOrigins { get; set; }

    public virtual DbSet<SingleOriginType> SingleOriginTypes { get; set; }

    public virtual DbSet<SpotsDim> SpotsDims { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VolumesDim> VolumesDims { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=192.168.0.189:5433;Database=tg_bot;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoriesDim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_dim_pk");

            entity.ToTable("categories_dim", "ttk_prod", tb => tb.HasComment("Категории товаров"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Category)
                .HasComment("Категория товара")
                .HasColumnType("character varying")
                .HasColumnName("category");
        });

        modelBuilder.Entity<CategoriesItemsDim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_dim_pk");

            entity.ToTable("categories_items_dim", "shipper_prod", tb => tb.HasComment("Категории товаров"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Category)
                .HasColumnType("character varying")
                .HasColumnName("category");
        });

        modelBuilder.Entity<ContainersDim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("containers_dim_pk");

            entity.ToTable("containers_dim", "ttk_prod", tb => tb.HasComment("Все возможный тары для товаров"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Container)
                .HasColumnType("character varying")
                .HasColumnName("container");
        });

        modelBuilder.Entity<DrinksTtk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("drinks_ttk_pk");

            entity.ToTable("drinks_ttk", "ttk_prod", tb => tb.HasComment("Основное ТТК по бару"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
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
            entity.Property(e => e.Ingridients)
                .HasColumnType("character varying")
                .HasColumnName("ingridients");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.PhotoPath)
                .HasComment("Содержит относительный путь к изображению в папке с проектом")
                .HasColumnType("character varying")
                .HasColumnName("photo_path");
            entity.Property(e => e.Prep)
                .HasComment("Заготовки (вместо blank)")
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
            entity.HasKey(e => e.Id).HasName("items_pk");

            entity.ToTable("items", "shipper_prod", tb => tb.HasComment("Продукция поставщиков"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Allergens)
                .HasColumnType("character varying")
                .HasColumnName("allergens");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.Carbohydrates).HasColumnName("carbohydrates");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Composition)
                .HasComment("Состав")
                .HasColumnType("character varying")
                .HasColumnName("composition");
            entity.Property(e => e.DairyFree).HasColumnName("dairy_free");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Energy).HasColumnName("energy");
            entity.Property(e => e.ExpirationDate)
                .HasComment("Срок годности")
                .HasColumnType("character varying")
                .HasColumnName("expiration_date");
            entity.Property(e => e.Fats).HasColumnName("fats");
            entity.Property(e => e.GlutenFree).HasColumnName("gluten_free");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Natural100)
                .HasComment("100% Натуральный!!! Эксперты в шоке!! Для мужского здоро...")
                .HasColumnName("natural100");
            entity.Property(e => e.PhotoPath)
                .HasColumnType("character varying")
                .HasColumnName("photo_path");
            entity.Property(e => e.Proteins).HasColumnName("proteins");
            entity.Property(e => e.ShipperId)
                .HasColumnType("character varying")
                .HasColumnName("shipper_id");
            entity.Property(e => e.SoyaFree).HasColumnName("soya_free");
            entity.Property(e => e.StorageCond)
                .HasComment("Условия хранения")
                .HasColumnType("character varying")
                .HasColumnName("storage_cond");
            entity.Property(e => e.SugarFree).HasColumnName("sugar_free");
            entity.Property(e => e.Vegan).HasColumnName("vegan");
            entity.Property(e => e.Weight).HasColumnName("weight");
        });

        modelBuilder.Entity<KbjuTtk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("kbju_ttk_pk");

            entity.ToTable("kbju_ttk", "ttk_prod", tb => tb.HasComment("КБЖУ ТТК"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Caffeine).HasColumnName("caffeine");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.Carbohydrates).HasColumnName("carbohydrates");
            entity.Property(e => e.Energy).HasColumnName("energy");
            entity.Property(e => e.Fats).HasColumnName("fats");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Proteins).HasColumnName("proteins");
            entity.Property(e => e.Variety)
                .HasColumnType("character varying")
                .HasColumnName("variety");
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shippres_pk");

            entity.ToTable("shippers", "shipper_prod", tb => tb.HasComment("Поставщики"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
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
                .HasComment("ФИО")
                .HasColumnType("character varying")
                .HasColumnName("full_name");
            entity.Property(e => e.Inn)
                .HasComment("Цифры какие-то")
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
            entity.HasKey(e => e.Id).HasName("newtable_pk");

            entity.ToTable("single_origins", "single_origin_prod", tb => tb.HasComment("Моносорта"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
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

        modelBuilder.Entity<SingleOriginType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("single_origin_types_pk");

            entity.ToTable("single_origin_types", "single_origin_prod", tb => tb.HasComment("Типы зерна"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Type)
                .HasColumnType("character varying")
                .HasColumnName("type");
        });

        modelBuilder.Entity<SpotsDim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("newtable_pk");

            entity.ToTable("spots_dim", "user_prod");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.City)
                .HasColumnType("character varying")
                .HasColumnName("city");
            entity.Property(e => e.Region)
                .HasColumnType("character varying")
                .HasColumnName("region");
            entity.Property(e => e.SpotName)
                .HasColumnType("character varying")
                .HasColumnName("spot_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pk");

            entity.ToTable("users", "user_prod", tb => tb.HasComment("Все пользователи, пользовавшиеся ботом"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.IsAccess).HasColumnName("is_access");
            entity.Property(e => e.IsAdmin).HasColumnName("is_admin");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
            entity.Property(e => e.Name)
                .HasComment("tg name")
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.SpotId).HasColumnName("spot_id");
            entity.Property(e => e.TgUserId).HasColumnName("tg_user_id");
        });

        modelBuilder.Entity<VolumesDim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("volume_dim_pk");

            entity.ToTable("volumes_dim", "ttk_prod", tb => tb.HasComment("Объемы тары"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Volume)
                .HasColumnType("character varying")
                .HasColumnName("volume");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
