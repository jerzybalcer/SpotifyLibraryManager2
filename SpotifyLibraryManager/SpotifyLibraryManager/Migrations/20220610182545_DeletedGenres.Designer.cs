// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpotifyLibraryManager.Database;

#nullable disable

namespace SpotifyLibraryManager.Migrations
{
    [DbContext(typeof(LibraryContext))]
    [Migration("20220610182545_DeletedGenres")]
    partial class DeletedGenres
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.5");

            modelBuilder.Entity("SpotifyLibraryManager.Models.Album", b =>
                {
                    b.Property<int>("AlbumId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CoverUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LikeDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("SpotifyUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalTracks")
                        .HasColumnType("INTEGER");

                    b.HasKey("AlbumId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("SpotifyLibraryManager.Models.Artist", b =>
                {
                    b.Property<int>("ArtistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AlbumId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ArtistId");

                    b.HasIndex("AlbumId");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("SpotifyLibraryManager.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AlbumId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ColorHex")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TagId");

                    b.HasIndex("AlbumId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("SpotifyLibraryManager.Models.Artist", b =>
                {
                    b.HasOne("SpotifyLibraryManager.Models.Album", null)
                        .WithMany("Artists")
                        .HasForeignKey("AlbumId");
                });

            modelBuilder.Entity("SpotifyLibraryManager.Models.Tag", b =>
                {
                    b.HasOne("SpotifyLibraryManager.Models.Album", null)
                        .WithMany("Tags")
                        .HasForeignKey("AlbumId");
                });

            modelBuilder.Entity("SpotifyLibraryManager.Models.Album", b =>
                {
                    b.Navigation("Artists");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
