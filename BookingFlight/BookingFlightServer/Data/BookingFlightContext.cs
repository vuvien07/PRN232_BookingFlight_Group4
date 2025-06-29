using System;
using System.Collections.Generic;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Data;

public partial class BookingFlightContext : DbContext
{
    public BookingFlightContext(DbContextOptions<BookingFlightContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<AirportPrice> AirportPrices { get; set; }

    public virtual DbSet<ClassSeat> ClassSeats { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightSeat> FlightSeats { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Plane> Planes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Supporter> Supporters { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketItem> TicketItems { get; set; }

    public virtual DbSet<UserLog> UserLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CDF08EEBBC");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(250)
                .HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnName("refresh_token_expiry_time");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__role_id__06CD04F7");

            entity.HasOne(d => d.Status).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__status___07C12930");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__43AA41414ABE5C5B");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.AccountId, "UQ__Admin__46A222CC12A283A6").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("phone_number");

            entity.HasOne(d => d.Account).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admin__account_i__08B54D69");
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.AirportId).HasName("PK__Airport__C795D516ECFCD54E");

            entity.ToTable("Airport");

            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.AirportCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("airport_code");
            entity.Property(e => e.AirportName)
                .HasMaxLength(50)
                .HasColumnName("airport_name");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.Airports)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Airport__manager__09A971A2");
        });

        modelBuilder.Entity<AirportPrice>(entity =>
        {
            entity.HasKey(e => e.AirportPriceId).HasName("PK__AirportP__E5C343472BA6DBE7");

            entity.ToTable("AirportPrice");

            entity.Property(e => e.AirportPriceId).HasColumnName("airport_price_id");
            entity.Property(e => e.AirportFromId).HasColumnName("airport_from_id");
            entity.Property(e => e.AirportToId).HasColumnName("airport_to_id");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("base_price");

            entity.HasOne(d => d.AirportFrom).WithMany(p => p.AirportPriceAirportFroms)
                .HasForeignKey(d => d.AirportFromId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AirportPr__airpo__0A9D95DB");

            entity.HasOne(d => d.AirportTo).WithMany(p => p.AirportPriceAirportTos)
                .HasForeignKey(d => d.AirportToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AirportPr__airpo__0B91BA14");
        });

        modelBuilder.Entity<ClassSeat>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__ClassSea__FDF47986BBF9332E");

            entity.ToTable("ClassSeat");

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("class_name");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId).HasName("PK__Complain__A771F61C03932033");

            entity.ToTable("Complaint");

            entity.Property(e => e.ComplaintId).HasColumnName("complaint_id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .HasColumnName("file_type");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .HasColumnName("file_url");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.SupporterId).HasColumnName("supporter_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Complaint__custo__0C85DE4D");

            entity.HasOne(d => d.Status).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Complaint__statu__0D7A0286");

            entity.HasOne(d => d.Supporter).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.SupporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Complaint__suppo__0E6E26BF");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB8539D884AF");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.AccountId, "UQ__Customer__46A222CC5A9E3C48").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("phone_number");

            entity.HasOne(d => d.Account).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customer__accoun__0F624AF8");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__BDBE9EF9A982EE6C");

            entity.ToTable("Discount");

            entity.HasIndex(e => e.DiscountCode, "UQ__Discount__75C1F0060ACA8870").IsUnique();

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DiscountCode)
                .HasMaxLength(50)
                .HasColumnName("discount_code");
            entity.Property(e => e.DiscountInfor).HasColumnName("discount_infor");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percent");
            entity.Property(e => e.DiscountTitle)
                .HasMaxLength(255)
                .HasColumnName("discount_title");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)1)
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discount__custom__10566F31");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__7A6B2B8C57939F9D");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Account).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__accoun__114A936A");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("PK__Flight__E3705765B435071C");

            entity.ToTable("Flight");

            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.ArrivalAirportId).HasColumnName("arrival_airport_id");
            entity.Property(e => e.ArrivalTime)
                .HasColumnType("datetime")
                .HasColumnName("arrival_time");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DepartureAirportId).HasColumnName("departure_airport_id");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("datetime")
                .HasColumnName("departure_time");
            entity.Property(e => e.FlightCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("flight_code");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.PlaneId).HasColumnName("plane_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Tax)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("tax");

            entity.HasOne(d => d.ArrivalAirport).WithMany(p => p.FlightArrivalAirports)
                .HasForeignKey(d => d.ArrivalAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__arrival___123EB7A3");

            entity.HasOne(d => d.Customer).WithMany(p => p.Flights)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__customer__1332DBDC");

            entity.HasOne(d => d.DepartureAirport).WithMany(p => p.FlightDepartureAirports)
                .HasForeignKey(d => d.DepartureAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__departur__14270015");

            entity.HasOne(d => d.Manager).WithMany(p => p.Flights)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__manager___151B244E");

            entity.HasOne(d => d.Plane).WithMany(p => p.Flights)
                .HasForeignKey(d => d.PlaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__plane_id__160F4887");

            entity.HasOne(d => d.Status).WithMany(p => p.Flights)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__status_i__17036CC0");

            entity.HasMany(d => d.Services).WithMany(p => p.Flights)
                .UsingEntity<Dictionary<string, object>>(
                    "FlightService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FlightSer__servi__1BC821DD"),
                    l => l.HasOne<Flight>().WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FlightSer__fligh__1AD3FDA4"),
                    j =>
                    {
                        j.HasKey("FlightId", "ServiceId").HasName("PK__FlightSe__00908CEF5EB538F0");
                        j.ToTable("FlightService");
                        j.IndexerProperty<int>("FlightId").HasColumnName("flight_id");
                        j.IndexerProperty<int>("ServiceId").HasColumnName("service_id");
                    });
        });

        modelBuilder.Entity<FlightSeat>(entity =>
        {
            entity.HasKey(e => new { e.FlightId, e.SeatId });

            entity.ToTable("Flight_Seat");

            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.IsSat).HasColumnName("isSat");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightSeats)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_Seat_Flight");

            entity.HasOne(d => d.Seat).WithMany(p => p.FlightSeats)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_Seat_Seat");

            entity.HasOne(d => d.Ticket).WithMany(p => p.FlightSeats)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_Flight_Seat_Ticket");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__52020FDDD3344EE9");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Detail)
                .HasMaxLength(100)
                .HasColumnName("detail");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasDefaultValue("anh-bien.jpg")
                .HasColumnName("image");
            entity.Property(e => e.ItemName)
                .HasMaxLength(50)
                .HasColumnName("item_name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Items)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Items_Status");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.ManagerId).HasName("PK__Manager__5A6073FC9DD59B5A");

            entity.ToTable("Manager");

            entity.HasIndex(e => e.AccountId, "UQ__Manager__46A222CCC5AC0D3A").IsUnique();

            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("phone_number");

            entity.HasOne(d => d.Account).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manager__account__1DB06A4F");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewId).HasName("PK__News__8215F588D5B064A0");

            entity.Property(e => e.NewId).HasColumnName("new_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Author)
                .HasMaxLength(50)
                .HasColumnName("author");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Account).WithMany(p => p.News)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__News__account_id__1EA48E88");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F1115864D");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Account).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__accou__1F98B2C1");

            entity.HasOne(d => d.Status).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__statu__208CD6FA");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EAAFF3D0C8");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__status___2180FB33");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__ticket___1EA48E88");
        });

        modelBuilder.Entity<Plane>(entity =>
        {
            entity.HasKey(e => e.PlaneId).HasName("PK__Plane__4D11D7FDEB6E940B");

            entity.ToTable("Plane");

            entity.Property(e => e.PlaneId).HasColumnName("plane_id");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Manufacture)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("manufacture");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("model");
            entity.Property(e => e.PlaneCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("plane_code");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.YearOfManufacture).HasColumnName("year_of_manufacture");

            entity.HasOne(d => d.Manager).WithMany(p => p.Planes)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Plane__manager_i__236943A5");

            entity.HasOne(d => d.Status).WithMany(p => p.Planes)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Plane__status_id__245D67DE");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CCEA7BBCED");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__906DED9C118FEE73");

            entity.ToTable("Seat");

            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.PlaneId).HasColumnName("plane_id");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(10)
                .HasColumnName("seat_number");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Class).WithMany(p => p.Seats)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__class_id__2180FB33");

            entity.HasOne(d => d.Plane).WithMany(p => p.Seats)
                .HasForeignKey(d => d.PlaneId)
                .HasConstraintName("FK_Seat_Plane");

            entity.HasOne(d => d.Status).WithMany(p => p.Seats)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__status_id__22751F6C");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__3E0DB8AF42ABCB6D");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Detail)
                .HasMaxLength(100)
                .HasColumnName("detail");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(50)
                .HasColumnName("service_name");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.Services)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Service__manager__2A164134");

            entity.HasOne(d => d.Status).WithMany(p => p.Services)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Service_Status");

            entity.HasMany(d => d.Items).WithMany(p => p.Services)
                .UsingEntity<Dictionary<string, object>>(
                    "ServiceItem",
                    r => r.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Service_I__item___2BFE89A6"),
                    l => l.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Service_I__servi__2CF2ADDF"),
                    j =>
                    {
                        j.HasKey("ServiceId", "ItemId").HasName("PK__Service___FB2D9852D6AE4DE4");
                        j.ToTable("Service_Item");
                        j.IndexerProperty<int>("ServiceId").HasColumnName("service_id");
                        j.IndexerProperty<int>("ItemId").HasColumnName("item_id");
                    });
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status__3683B531CC8A8C83");

            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
            entity.Property(e => e.StatusType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("status_type");
        });

        modelBuilder.Entity<Supporter>(entity =>
        {
            entity.HasKey(e => e.SupporterId).HasName("PK__Supporte__F3A5770137213A14");

            entity.ToTable("Supporter");

            entity.HasIndex(e => e.AccountId, "UQ__Supporte__46A222CCDE9423FB").IsUnique();

            entity.Property(e => e.SupporterId).HasColumnName("supporter_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasColumnName("phone_number");

            entity.HasOne(d => d.Account).WithOne(p => p.Supporter)
                .HasForeignKey<Supporter>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Supporter__accou__2DE6D218");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__D596F96BCC0A6FCE");

            entity.ToTable("Ticket");

            entity.HasIndex(e => e.TicketNumber, "UQ__Ticket__413613D2A41FFAC9").IsUnique();

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.BookingDate).HasColumnName("booking_date");
            entity.Property(e => e.ClassSeatId).HasColumnName("class_seat_id");
            entity.Property(e => e.ContactAddress)
                .HasMaxLength(255)
                .HasColumnName("contact_address");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactFullName)
                .HasMaxLength(100)
                .HasColumnName("contact_full_name");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(10)
                .HasColumnName("contact_phone");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TicketNumber)
                .HasMaxLength(20)
                .HasColumnName("ticket_number");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.ClassSeat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ClassSeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_ClassSeat");

            entity.HasOne(d => d.Customer).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Ticket__customer__2B0A656D");

            entity.HasOne(d => d.Flight).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__flight_i__2BFE89A6");

            entity.HasOne(d => d.Status).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__status_i__2CF2ADDF");
        });

        modelBuilder.Entity<TicketItem>(entity =>
        {
            entity.HasKey(e => new { e.TicketId, e.ItemId });

            entity.ToTable("Ticket_Item");

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Item).WithMany(p => p.TicketItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Item_Items");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketItems)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Item_Ticket");
        });

        modelBuilder.Entity<UserLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__User_Log__9E2397E0F8B8CA68");

            entity.ToTable("User_Logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.AccessTime)
                .HasColumnType("datetime")
                .HasColumnName("access_time");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Detail)
                .HasMaxLength(50)
                .HasColumnName("detail");

            entity.HasOne(d => d.Account).WithMany(p => p.UserLogs)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Logs__accou__32AB8735");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
