using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinesscardsApi.Migrations
{
    /// <inheritdoc />
    public partial class change1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessCards",
                table: "BusinessCards");

            migrationBuilder.RenameTable(
                name: "BusinessCards",
                newName: "business_cards");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "business_cards",
                newName: "website");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "business_cards",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "business_cards",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "business_cards",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "business_cards",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "business_cards",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "business_cards",
                newName: "profile_image_url");

            migrationBuilder.RenameColumn(
                name: "PersonalBlogUrl",
                table: "business_cards",
                newName: "personal_blog_url");

            migrationBuilder.RenameColumn(
                name: "LinkedInUrl",
                table: "business_cards",
                newName: "linked_in_url");

            migrationBuilder.RenameColumn(
                name: "GitHubUrl",
                table: "business_cards",
                newName: "git_hub_url");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "business_cards",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "business_cards",
                newName: "company_name");

            migrationBuilder.AddPrimaryKey(
                name: "pk_business_cards",
                table: "business_cards",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_business_cards",
                table: "business_cards");

            migrationBuilder.RenameTable(
                name: "business_cards",
                newName: "BusinessCards");

            migrationBuilder.RenameColumn(
                name: "website",
                table: "BusinessCards",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "BusinessCards",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "BusinessCards",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "BusinessCards",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "BusinessCards",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "BusinessCards",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "profile_image_url",
                table: "BusinessCards",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "personal_blog_url",
                table: "BusinessCards",
                newName: "PersonalBlogUrl");

            migrationBuilder.RenameColumn(
                name: "linked_in_url",
                table: "BusinessCards",
                newName: "LinkedInUrl");

            migrationBuilder.RenameColumn(
                name: "git_hub_url",
                table: "BusinessCards",
                newName: "GitHubUrl");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "BusinessCards",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "company_name",
                table: "BusinessCards",
                newName: "CompanyName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessCards",
                table: "BusinessCards",
                column: "Id");
        }
    }
}
