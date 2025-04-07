using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTLWNCao.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CongTys",
                columns: table => new
                {
                    MaCongTy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCongTy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ThongTinCongTy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongTys", x => x.MaCongTy);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    MaUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenUser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.MaUser);
                });

            migrationBuilder.CreateTable(
                name: "UserCongTys",
                columns: table => new
                {
                    MaUserCongTy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongTy = table.Column<int>(type: "int", nullable: false),
                    MaUser = table.Column<int>(type: "int", nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCongTys", x => x.MaUserCongTy);
                    table.ForeignKey(
                        name: "FK_UserCongTys_CongTys_MaCongTy",
                        column: x => x.MaCongTy,
                        principalTable: "CongTys",
                        principalColumn: "MaCongTy",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCongTys_Users_MaUser",
                        column: x => x.MaUser,
                        principalTable: "Users",
                        principalColumn: "MaUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DuAns",
                columns: table => new
                {
                    MaDuAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaUserCongTy = table.Column<int>(type: "int", nullable: false),
                    MaCongTy = table.Column<int>(type: "int", nullable: false),
                    TenDuAn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDungDuAn = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuAns", x => x.MaDuAn);
                    table.ForeignKey(
                        name: "FK_DuAns_CongTys_MaCongTy",
                        column: x => x.MaCongTy,
                        principalTable: "CongTys",
                        principalColumn: "MaCongTy",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuAns_UserCongTys_MaUserCongTy",
                        column: x => x.MaUserCongTy,
                        principalTable: "UserCongTys",
                        principalColumn: "MaUserCongTy",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhomChats",
                columns: table => new
                {
                    MaNhomChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaUserCongTy = table.Column<int>(type: "int", nullable: false),
                    TenNhomChat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomChats", x => x.MaNhomChat);
                    table.ForeignKey(
                        name: "FK_NhomChats_UserCongTys_MaUserCongTy",
                        column: x => x.MaUserCongTy,
                        principalTable: "UserCongTys",
                        principalColumn: "MaUserCongTy",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhanCongCongViecs",
                columns: table => new
                {
                    MaCongViec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaUserCongTy = table.Column<int>(type: "int", nullable: false),
                    TenCongViec = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDung = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Anh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileTaiLieu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaCongTy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongCongViecs", x => x.MaCongViec);
                    table.ForeignKey(
                        name: "FK_PhanCongCongViecs_CongTys_MaCongTy",
                        column: x => x.MaCongTy,
                        principalTable: "CongTys",
                        principalColumn: "MaCongTy",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanCongCongViecs_DuAns_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DuAns",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhanCongCongViecs_UserCongTys_MaUserCongTy",
                        column: x => x.MaUserCongTy,
                        principalTable: "UserCongTys",
                        principalColumn: "MaUserCongTy",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNhomChats",
                columns: table => new
                {
                    MaUserNhomChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhomChat = table.Column<int>(type: "int", nullable: false),
                    MaUserCongTy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNhomChats", x => x.MaUserNhomChat);
                    table.ForeignKey(
                        name: "FK_UserNhomChats_NhomChats_MaNhomChat",
                        column: x => x.MaNhomChat,
                        principalTable: "NhomChats",
                        principalColumn: "MaNhomChat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserNhomChats_UserCongTys_MaUserCongTy",
                        column: x => x.MaUserCongTy,
                        principalTable: "UserCongTys",
                        principalColumn: "MaUserCongTy",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TinNhans",
                columns: table => new
                {
                    MaChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaUserNhomChat = table.Column<int>(type: "int", nullable: false),
                    MaNhomChat = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Anh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileTaiLieu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinNhans", x => x.MaChat);
                    table.ForeignKey(
                        name: "FK_TinNhans_NhomChats_MaNhomChat",
                        column: x => x.MaNhomChat,
                        principalTable: "NhomChats",
                        principalColumn: "MaNhomChat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TinNhans_UserNhomChats_MaUserNhomChat",
                        column: x => x.MaUserNhomChat,
                        principalTable: "UserNhomChats",
                        principalColumn: "MaUserNhomChat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuAns_MaCongTy",
                table: "DuAns",
                column: "MaCongTy");

            migrationBuilder.CreateIndex(
                name: "IX_DuAns_MaUserCongTy",
                table: "DuAns",
                column: "MaUserCongTy");

            migrationBuilder.CreateIndex(
                name: "IX_NhomChats_MaUserCongTy",
                table: "NhomChats",
                column: "MaUserCongTy");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongCongViecs_MaCongTy",
                table: "PhanCongCongViecs",
                column: "MaCongTy");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongCongViecs_MaDuAn",
                table: "PhanCongCongViecs",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongCongViecs_MaUserCongTy",
                table: "PhanCongCongViecs",
                column: "MaUserCongTy");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_MaNhomChat",
                table: "TinNhans",
                column: "MaNhomChat");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_MaUserNhomChat",
                table: "TinNhans",
                column: "MaUserNhomChat");

            migrationBuilder.CreateIndex(
                name: "IX_UserCongTys_MaCongTy",
                table: "UserCongTys",
                column: "MaCongTy");

            migrationBuilder.CreateIndex(
                name: "IX_UserCongTys_MaUser",
                table: "UserCongTys",
                column: "MaUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserNhomChats_MaNhomChat",
                table: "UserNhomChats",
                column: "MaNhomChat");

            migrationBuilder.CreateIndex(
                name: "IX_UserNhomChats_MaUserCongTy",
                table: "UserNhomChats",
                column: "MaUserCongTy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhanCongCongViecs");

            migrationBuilder.DropTable(
                name: "TinNhans");

            migrationBuilder.DropTable(
                name: "DuAns");

            migrationBuilder.DropTable(
                name: "UserNhomChats");

            migrationBuilder.DropTable(
                name: "NhomChats");

            migrationBuilder.DropTable(
                name: "UserCongTys");

            migrationBuilder.DropTable(
                name: "CongTys");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
