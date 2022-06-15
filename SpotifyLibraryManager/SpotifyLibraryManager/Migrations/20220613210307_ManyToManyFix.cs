using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyLibraryManager.Migrations
{
    public partial class ManyToManyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Albums_AlbumId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Albums_AlbumId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_AlbumId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Artists_AlbumId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "Artists");

            migrationBuilder.CreateTable(
                name: "AlbumArtist",
                columns: table => new
                {
                    AlbumsAlbumId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArtistsArtistId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumArtist", x => new { x.AlbumsAlbumId, x.ArtistsArtistId });
                    table.ForeignKey(
                        name: "FK_AlbumArtist_Albums_AlbumsAlbumId",
                        column: x => x.AlbumsAlbumId,
                        principalTable: "Albums",
                        principalColumn: "AlbumId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumArtist_Artists_ArtistsArtistId",
                        column: x => x.ArtistsArtistId,
                        principalTable: "Artists",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumTag",
                columns: table => new
                {
                    AlbumsAlbumId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsTagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumTag", x => new { x.AlbumsAlbumId, x.TagsTagId });
                    table.ForeignKey(
                        name: "FK_AlbumTag_Albums_AlbumsAlbumId",
                        column: x => x.AlbumsAlbumId,
                        principalTable: "Albums",
                        principalColumn: "AlbumId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumTag_Tags_TagsTagId",
                        column: x => x.TagsTagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtist_ArtistsArtistId",
                table: "AlbumArtist",
                column: "ArtistsArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumTag_TagsTagId",
                table: "AlbumTag",
                column: "TagsTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumArtist");

            migrationBuilder.DropTable(
                name: "AlbumTag");

            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "Artists",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AlbumId",
                table: "Tags",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_AlbumId",
                table: "Artists",
                column: "AlbumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Albums_AlbumId",
                table: "Artists",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "AlbumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Albums_AlbumId",
                table: "Tags",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "AlbumId");
        }
    }
}
