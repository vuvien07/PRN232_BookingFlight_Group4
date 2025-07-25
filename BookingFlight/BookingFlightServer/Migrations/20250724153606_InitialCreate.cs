using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingFlightServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassSeat",
                columns: table => new
                {
                    class_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    class_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClassSea__FDF479865214DBFF", x => x.class_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__760965CC9AF58AE3", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status_type = table.Column<string>(type: "char(50)", unicode: false, fixedLength: true, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Status__3683B5310291BB56", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__46A222CD16D48287", x => x.account_id);
                    table.ForeignKey(
                        name: "FK__Account__role_id__07C12930",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id");
                    table.ForeignKey(
                        name: "FK__Account__status___08B54D69",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    item_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    detail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    price = table.Column<int>(type: "int", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: true),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValue: "anh-bien.jpg")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Items__52020FDD275EA735", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_Items_Status",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "varchar(17)", unicode: false, maxLength: 17, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__43AA41410978B09D", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK__Admin__account_i__09A971A2",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "varchar(17)", unicode: false, maxLength: 17, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__CD65CB8518A1C4A0", x => x.customer_id);
                    table.ForeignKey(
                        name: "FK__Customer__accoun__10566F31",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    create_at = table.Column<DateOnly>(type: "date", nullable: true),
                    account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__7A6B2B8CD8086017", x => x.feedback_id);
                    table.ForeignKey(
                        name: "FK__Feedback__accoun__123EB7A3",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    manager_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "varchar(17)", unicode: false, maxLength: 17, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Manager__5A6073FC44C27C13", x => x.manager_id);
                    table.ForeignKey(
                        name: "FK__Manager__account__1DB06A4F",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    new_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    author = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__News__8215F58893AC1690", x => x.new_id);
                    table.ForeignKey(
                        name: "FK__News__account_id__1EA48E88",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    create_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__E059842FCF900CB8", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__Notificat__accou__1F98B2C1",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                    table.ForeignKey(
                        name: "FK__Notificat__statu__208CD6FA",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Supporter",
                columns: table => new
                {
                    supporter_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "varchar(17)", unicode: false, maxLength: 17, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supporte__F3A577016AD0363F", x => x.supporter_id);
                    table.ForeignKey(
                        name: "FK__Supporter__accou__2BFE89A6",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "User_Logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    detail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    access_time = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Log__9E2397E09BCC7CA8", x => x.log_id);
                    table.ForeignKey(
                        name: "FK__User_Logs__accou__32AB8735",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    discount_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    discount_percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    discount_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    discount_infor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)1),
                    customer_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Discount__BDBE9EF987994503", x => x.discount_id);
                    table.ForeignKey(
                        name: "FK__Discount__custom__114A936A",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "customer_id");
                });

            migrationBuilder.CreateTable(
                name: "Airport",
                columns: table => new
                {
                    airport_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    airport_code = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    airport_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    city = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    manager_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Airport__C795D5168D90093F", x => x.airport_id);
                    table.ForeignKey(
                        name: "FK__Airport__manager__0A9D95DB",
                        column: x => x.manager_id,
                        principalTable: "Manager",
                        principalColumn: "manager_id");
                });

            migrationBuilder.CreateTable(
                name: "Plane",
                columns: table => new
                {
                    plane_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plane_code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    manufacture = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    year_of_manufacture = table.Column<int>(type: "int", nullable: false),
                    manager_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Plane__4D11D7FDD690FD48", x => x.plane_id);
                    table.ForeignKey(
                        name: "FK__Plane__manager_i__236943A5",
                        column: x => x.manager_id,
                        principalTable: "Manager",
                        principalColumn: "manager_id");
                    table.ForeignKey(
                        name: "FK__Plane__status_id__245D67DE",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    service_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    detail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    manager_id = table.Column<int>(type: "int", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service__3E0DB8AFAC8D5893", x => x.service_id);
                    table.ForeignKey(
                        name: "FK_Service_Status",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK__Service__manager__282DF8C2",
                        column: x => x.manager_id,
                        principalTable: "Manager",
                        principalColumn: "manager_id");
                });

            migrationBuilder.CreateTable(
                name: "Complaint",
                columns: table => new
                {
                    complaint_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    supporter_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    create_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    file_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    file_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Complain__A771F61CEAA299EB", x => x.complaint_id);
                    table.ForeignKey(
                        name: "FK__Complaint__custo__0D7A0286",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "FK__Complaint__statu__0E6E26BF",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK__Complaint__suppo__0F624AF8",
                        column: x => x.supporter_id,
                        principalTable: "Supporter",
                        principalColumn: "supporter_id");
                });

            migrationBuilder.CreateTable(
                name: "AirportPrice",
                columns: table => new
                {
                    airport_price_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    airport_from_id = table.Column<int>(type: "int", nullable: false),
                    airport_to_id = table.Column<int>(type: "int", nullable: false),
                    base_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AirportP__E5C343471C893DA2", x => x.airport_price_id);
                    table.ForeignKey(
                        name: "FK__AirportPr__airpo__0B91BA14",
                        column: x => x.airport_from_id,
                        principalTable: "Airport",
                        principalColumn: "airport_id");
                    table.ForeignKey(
                        name: "FK__AirportPr__airpo__0C85DE4D",
                        column: x => x.airport_to_id,
                        principalTable: "Airport",
                        principalColumn: "airport_id");
                });

            migrationBuilder.CreateTable(
                name: "Flight",
                columns: table => new
                {
                    flight_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    flight_code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    tax = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    departure_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    arrival_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    plane_id = table.Column<int>(type: "int", nullable: false),
                    manager_id = table.Column<int>(type: "int", nullable: false),
                    departure_airport_id = table.Column<int>(type: "int", nullable: false),
                    arrival_airport_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Flight__E3705765B435071C", x => x.flight_id);
                    table.ForeignKey(
                        name: "FK__Flight__arrival___123EB7A3",
                        column: x => x.arrival_airport_id,
                        principalTable: "Airport",
                        principalColumn: "airport_id");
                    table.ForeignKey(
                        name: "FK__Flight__departur__14270015",
                        column: x => x.departure_airport_id,
                        principalTable: "Airport",
                        principalColumn: "airport_id");
                    table.ForeignKey(
                        name: "FK__Flight__manager___151B244E",
                        column: x => x.manager_id,
                        principalTable: "Manager",
                        principalColumn: "manager_id");
                    table.ForeignKey(
                        name: "FK__Flight__plane_id__160F4887",
                        column: x => x.plane_id,
                        principalTable: "Plane",
                        principalColumn: "plane_id");
                    table.ForeignKey(
                        name: "FK__Flight__status_i__17036CC0",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Seat",
                columns: table => new
                {
                    seat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    seat_number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    class_id = table.Column<int>(type: "int", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    plane_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seat__906DED9C118FEE73", x => x.seat_id);
                    table.ForeignKey(
                        name: "FK_Seat_Plane",
                        column: x => x.plane_id,
                        principalTable: "Plane",
                        principalColumn: "plane_id");
                    table.ForeignKey(
                        name: "FK__Seat__class_id__2180FB33",
                        column: x => x.class_id,
                        principalTable: "ClassSeat",
                        principalColumn: "class_id");
                    table.ForeignKey(
                        name: "FK__Seat__status_id__22751F6C",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Service_Item",
                columns: table => new
                {
                    service_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service___FB2D9852CD5AA2FD", x => new { x.service_id, x.item_id });
                    table.ForeignKey(
                        name: "FK__Service_I__item___2A164134",
                        column: x => x.item_id,
                        principalTable: "Items",
                        principalColumn: "item_id");
                    table.ForeignKey(
                        name: "FK__Service_I__servi__2B0A656D",
                        column: x => x.service_id,
                        principalTable: "Service",
                        principalColumn: "service_id");
                });

            migrationBuilder.CreateTable(
                name: "FlightService",
                columns: table => new
                {
                    flight_id = table.Column<int>(type: "int", nullable: false),
                    service_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FlightSe__00908CEFF11099F1", x => new { x.flight_id, x.service_id });
                    table.ForeignKey(
                        name: "FK__FlightSer__fligh__1AD3FDA4",
                        column: x => x.flight_id,
                        principalTable: "Flight",
                        principalColumn: "flight_id");
                    table.ForeignKey(
                        name: "FK__FlightSer__servi__1BC821DD",
                        column: x => x.service_id,
                        principalTable: "Service",
                        principalColumn: "service_id");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    ticket_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ticket_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    flight_id = table.Column<int>(type: "int", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    booking_date = table.Column<DateOnly>(type: "date", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    total_price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    class_seat_id = table.Column<int>(type: "int", nullable: false),
                    contact_full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    contact_phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    contact_email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    contact_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ticket__D596F96BCC0A6FCE", x => x.ticket_id);
                    table.ForeignKey(
                        name: "FK_Ticket_ClassSeat",
                        column: x => x.class_seat_id,
                        principalTable: "ClassSeat",
                        principalColumn: "class_id");
                    table.ForeignKey(
                        name: "FK__Ticket__customer__2B0A656D",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "FK__Ticket__flight_i__2BFE89A6",
                        column: x => x.flight_id,
                        principalTable: "Flight",
                        principalColumn: "flight_id");
                    table.ForeignKey(
                        name: "FK__Ticket__status_i__2CF2ADDF",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "FeedbackTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedbackId = table.Column<int>(type: "int", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackTickets_Feedback_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedback",
                        principalColumn: "feedback_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackTickets_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "ticket_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flight_Seat",
                columns: table => new
                {
                    flight_id = table.Column<int>(type: "int", nullable: false),
                    seat_id = table.Column<int>(type: "int", nullable: false),
                    isSat = table.Column<bool>(type: "bit", nullable: false),
                    ticket_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flight_Seat", x => new { x.flight_id, x.seat_id });
                    table.ForeignKey(
                        name: "FK_Flight_Seat_Flight",
                        column: x => x.flight_id,
                        principalTable: "Flight",
                        principalColumn: "flight_id");
                    table.ForeignKey(
                        name: "FK_Flight_Seat_Seat",
                        column: x => x.seat_id,
                        principalTable: "Seat",
                        principalColumn: "seat_id");
                    table.ForeignKey(
                        name: "FK_Flight_Seat_Ticket",
                        column: x => x.ticket_id,
                        principalTable: "Ticket",
                        principalColumn: "ticket_id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    price = table.Column<int>(type: "int", nullable: false),
                    payment_method = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ticket_id = table.Column<int>(type: "int", nullable: false),
                    payment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__ED1FC9EA3B39579E", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK__Payment__status___2180FB33",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK__Payment__ticket___1EA48E88",
                        column: x => x.ticket_id,
                        principalTable: "Ticket",
                        principalColumn: "ticket_id");
                });

            migrationBuilder.CreateTable(
                name: "Ticket_Item",
                columns: table => new
                {
                    ticket_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket_Item", x => new { x.ticket_id, x.item_id });
                    table.ForeignKey(
                        name: "FK_Ticket_Item_Items",
                        column: x => x.item_id,
                        principalTable: "Items",
                        principalColumn: "item_id");
                    table.ForeignKey(
                        name: "FK_Ticket_Item_Ticket",
                        column: x => x.ticket_id,
                        principalTable: "Ticket",
                        principalColumn: "ticket_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_role_id",
                table: "Account",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Account_status_id",
                table: "Account",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Admin__46A222CC24D6D9F3",
                table: "Admin",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Airport_manager_id",
                table: "Airport",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_AirportPrice_airport_from_id",
                table: "AirportPrice",
                column: "airport_from_id");

            migrationBuilder.CreateIndex(
                name: "IX_AirportPrice_airport_to_id",
                table: "AirportPrice",
                column: "airport_to_id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_customer_id",
                table: "Complaint",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_status_id",
                table: "Complaint",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_supporter_id",
                table: "Complaint",
                column: "supporter_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__46A222CC00BC4C29",
                table: "Customer",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discount_customer_id",
                table: "Discount",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Discount__75C1F006A23F89FF",
                table: "Discount",
                column: "discount_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_account_id",
                table: "Feedback",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackTickets_FeedbackId",
                table: "FeedbackTickets",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackTickets_TicketId",
                table: "FeedbackTickets",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_arrival_airport_id",
                table: "Flight",
                column: "arrival_airport_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_departure_airport_id",
                table: "Flight",
                column: "departure_airport_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_manager_id",
                table: "Flight",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_plane_id",
                table: "Flight",
                column: "plane_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_status_id",
                table: "Flight",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_Seat_seat_id",
                table: "Flight_Seat",
                column: "seat_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_Seat_ticket_id",
                table: "Flight_Seat",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_FlightService_service_id",
                table: "FlightService",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_Items_status_id",
                table: "Items",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Manager__46A222CC0AB30127",
                table: "Manager",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_News_account_id",
                table: "News",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_account_id",
                table: "Notification",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_status_id",
                table: "Notification",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_status_id",
                table: "Payment",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ticket_id",
                table: "Payment",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_Plane_manager_id",
                table: "Plane",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_Plane_status_id",
                table: "Plane",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_class_id",
                table: "Seat",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_plane_id",
                table: "Seat",
                column: "plane_id");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_status_id",
                table: "Seat",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Service_manager_id",
                table: "Service",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_Service_status_id",
                table: "Service",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Item_item_id",
                table: "Service_Item",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Supporte__46A222CCCEC18A66",
                table: "Supporter",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_class_seat_id",
                table: "Ticket",
                column: "class_seat_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_customer_id",
                table: "Ticket",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_flight_id",
                table: "Ticket",
                column: "flight_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_status_id",
                table: "Ticket",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Ticket__413613D2A41FFAC9",
                table: "Ticket",
                column: "ticket_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Item_item_id",
                table: "Ticket_Item",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Logs_account_id",
                table: "User_Logs",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "AirportPrice");

            migrationBuilder.DropTable(
                name: "Complaint");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "FeedbackTickets");

            migrationBuilder.DropTable(
                name: "Flight_Seat");

            migrationBuilder.DropTable(
                name: "FlightService");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Service_Item");

            migrationBuilder.DropTable(
                name: "Ticket_Item");

            migrationBuilder.DropTable(
                name: "User_Logs");

            migrationBuilder.DropTable(
                name: "Supporter");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "ClassSeat");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Flight");

            migrationBuilder.DropTable(
                name: "Airport");

            migrationBuilder.DropTable(
                name: "Plane");

            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Status");
        }
    }
}
