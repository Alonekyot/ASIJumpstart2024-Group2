using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoomBooking.Data.Migrations
{
    /// <inheritdoc />
    public partial class room_initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomCapacity = table.Column<int>(type: "int", nullable: false),
                    Audio = table.Column<bool>(type: "bit", nullable: false),
                    Video = table.Column<bool>(type: "bit", nullable: false),
                    WhiteBoard = table.Column<bool>(type: "bit", nullable: false),
                    Projector = table.Column<bool>(type: "bit", nullable: false),
                    Loudspeaker = table.Column<bool>(type: "bit", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
