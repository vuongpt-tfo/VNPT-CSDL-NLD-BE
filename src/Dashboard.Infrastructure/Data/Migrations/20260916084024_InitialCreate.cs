using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dashboard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dashboard_sch");

            migrationBuilder.CreateTable(
                name: "dim_administrative_unit",
                schema: "dashboard_sch",
                columns: table => new
                {
                    administrative_unit_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    parent_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    unit_type_id = table.Column<short>(type: "smallint", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    effective_from = table.Column<int>(type: "integer", nullable: false),
                    effective_to = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_administrative_unit", x => x.administrative_unit_id);
                });

            migrationBuilder.CreateTable(
                name: "dim_age_range",
                schema: "dashboard_sch",
                columns: table => new
                {
                    age_range_id = table.Column<int>(type: "integer", nullable: false),
                    age_from = table.Column<short>(type: "smallint", nullable: false),
                    age_to = table.Column<short>(type: "smallint", nullable: false),
                    label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    colour = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_age_range", x => x.age_range_id);
                });

            migrationBuilder.CreateTable(
                name: "dim_analytical",
                schema: "dashboard_sch",
                columns: table => new
                {
                    analytical_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    analytical_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    dimension_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dim_analytical_pkey", x => x.analytical_id);
                });

            migrationBuilder.CreateTable(
                name: "dim_catalog_items",
                schema: "dashboard_sch",
                columns: table => new
                {
                    catalog_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    effective_from = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    effective_to = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_catalog_items", x => new { x.catalog_name, x.code, x.effective_from });
                });

            migrationBuilder.CreateTable(
                name: "dim_industry",
                schema: "dashboard_sch",
                columns: table => new
                {
                    industry_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    parent_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    level = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    effective_from = table.Column<int>(type: "integer", nullable: false),
                    effective_to = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_industry", x => x.industry_id);
                });

            migrationBuilder.CreateTable(
                name: "dim_occupation",
                schema: "dashboard_sch",
                columns: table => new
                {
                    occupation_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    parent_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    level = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    effective_from = table.Column<int>(type: "integer", nullable: false),
                    effective_to = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_occupation", x => x.occupation_id);
                });

            migrationBuilder.CreateTable(
                name: "labour_economic_status_summary",
                schema: "dashboard_sch",
                columns: table => new
                {
                    summary_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reference_period = table.Column<DateOnly>(type: "date", nullable: false),
                    administrative_unit_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    education_level_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    technical_level_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    economic_status_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    labour_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    by_priority = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    by_gender = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    by_industry = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    by_age_group = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    source_identity_snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    source_employment_snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    analytical_definition_version = table.Column<int>(type: "integer", nullable: true),
                    processing_run_id = table.Column<long>(type: "bigint", nullable: true),
                    aggregated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("labour_economic_status_summary_pkey", x => x.summary_id);
                });

            migrationBuilder.CreateTable(
                name: "labour_employment_snapshot",
                schema: "dashboard_sch",
                columns: table => new
                {
                    labour_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    social_insurance_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    education_level_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    technical_level_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    major_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    major_description = table.Column<string>(type: "text", nullable: true),
                    skill_industry_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    skill_level_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    economic_activities_code = table.Column<short>(type: "smallint", nullable: false),
                    position_groups_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    occupation_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    contract_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    employment_status_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    work_location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    job_types_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    job_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    total_work_experience = table.Column<int>(type: "integer", nullable: true),
                    number_of_jobs = table.Column<int>(type: "integer", nullable: true),
                    employer_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    industry_codes = table.Column<string[]>(type: "text[]", nullable: true),
                    industrial_zone_status = table.Column<bool>(type: "boolean", nullable: true),
                    employer_sectors_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ever_worked = table.Column<bool>(type: "boolean", nullable: true),
                    unemployment_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    unemployment_duration = table.Column<int>(type: "integer", nullable: true),
                    unemployment_benefit_status = table.Column<bool>(type: "boolean", nullable: true),
                    unemployment_reason = table.Column<string>(type: "text", nullable: true),
                    job_search_wanted = table.Column<bool>(type: "boolean", nullable: true),
                    job_search_status = table.Column<string>(type: "text", nullable: true),
                    uneconomic_reason_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    participation_form_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    si_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    si_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    priority_codes = table.Column<string[]>(type: "text[]", nullable: true),
                    source_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    snapshot_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labour_employment_snapshot", x => x.labour_id);
                });

            migrationBuilder.CreateTable(
                name: "labour_identity_snapshot",
                schema: "dashboard_sch",
                columns: table => new
                {
                    labour_id = table.Column<long>(type: "bigint", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cccd_number = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    cmnd_number = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    acc_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    gender_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issue_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    issue_place = table.Column<string>(type: "text", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ethnic_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    religion_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    permanent_address_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    permanent_address_detail = table.Column<string>(type: "text", nullable: true),
                    current_address_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    current_address_detail = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    marital_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    source_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    snapshot_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labour_identity_snapshot", x => new { x.labour_id, x.date_of_birth });
                });

            migrationBuilder.CreateTable(
                name: "labour_identity_summary",
                schema: "dashboard_sch",
                columns: table => new
                {
                    summary_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    administrative_unit_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    education_level_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    technical_level_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    top_level_occupation_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    age_group_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    economic_status_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    labour_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    by_priority = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    male_count = table.Column<long>(type: "bigint", nullable: true),
                    by_industry = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    source_identity_snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    source_employment_snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    analytical_definition_version = table.Column<int>(type: "integer", nullable: true),
                    processing_run_id = table.Column<long>(type: "bigint", nullable: true),
                    aggregated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("labour_identity_summary_pkey", x => x.summary_id);
                });

            migrationBuilder.CreateTable(
                name: "labour_time_summary",
                schema: "dashboard_sch",
                columns: table => new
                {
                    summary_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reference_period = table.Column<DateOnly>(type: "date", nullable: false),
                    administrative_unit_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    education_level_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    technical_level_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    working_age_labour_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    out_working_age_labour_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_labour_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    previous_working_age_labour_count = table.Column<long>(type: "bigint", nullable: true),
                    previous_out_working_age_labour_count = table.Column<long>(type: "bigint", nullable: true),
                    previous_total_labour_count = table.Column<long>(type: "bigint", nullable: true),
                    is_revised = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    revised_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    analytical_definition_version = table.Column<int>(type: "integer", nullable: true),
                    working_age_from = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)15),
                    working_age_to = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)62),
                    source_identity_snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    source_employment_snapshot_version = table.Column<long>(type: "bigint", nullable: false),
                    processing_run_id = table.Column<long>(type: "bigint", nullable: true),
                    aggregated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("labour_time_summary_pkey", x => x.summary_id);
                });

            migrationBuilder.CreateTable(
                name: "dim_analytical_item",
                schema: "dashboard_sch",
                columns: table => new
                {
                    analytical_item_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    analytical_id = table.Column<long>(type: "bigint", nullable: false),
                    item_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    item_value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    parent_item_id = table.Column<long>(type: "bigint", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    range_from = table.Column<decimal>(type: "numeric", nullable: true),
                    range_to = table.Column<decimal>(type: "numeric", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dim_analytical_item_pkey", x => x.analytical_item_id);
                    table.ForeignKey(
                        name: "fk_dim_analytical_item_analytical",
                        column: x => x.analytical_id,
                        principalSchema: "dashboard_sch",
                        principalTable: "dim_analytical",
                        principalColumn: "analytical_id");
                    table.ForeignKey(
                        name: "fk_dim_analytical_item_parent",
                        column: x => x.parent_item_id,
                        principalSchema: "dashboard_sch",
                        principalTable: "dim_analytical_item",
                        principalColumn: "analytical_item_id");
                });

            migrationBuilder.CreateTable(
                name: "dim_analytical_rule",
                schema: "dashboard_sch",
                columns: table => new
                {
                    analytical_rule_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    analytical_id = table.Column<long>(type: "bigint", nullable: false),
                    rule_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    configuration = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dim_analytical_rule_pkey", x => x.analytical_rule_id);
                    table.ForeignKey(
                        name: "fk_analytical_rule_analytical",
                        column: x => x.analytical_id,
                        principalSchema: "dashboard_sch",
                        principalTable: "dim_analytical",
                        principalColumn: "analytical_id");
                });

            migrationBuilder.CreateTable(
                name: "dim_analytical_item_translation",
                schema: "dashboard_sch",
                columns: table => new
                {
                    translation_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    analytical_item_id = table.Column<long>(type: "bigint", nullable: false),
                    language_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    item_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dim_analytical_item_translation_pkey", x => x.translation_id);
                    table.ForeignKey(
                        name: "fk_analytical_item_translation_item",
                        column: x => x.analytical_item_id,
                        principalSchema: "dashboard_sch",
                        principalTable: "dim_analytical_item",
                        principalColumn: "analytical_item_id");
                });

            migrationBuilder.CreateTable(
                name: "dim_analytical_rule_condition",
                schema: "dashboard_sch",
                columns: table => new
                {
                    analytical_rule_condition_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    analytical_rule_id = table.Column<long>(type: "bigint", nullable: false),
                    analytical_item_id = table.Column<long>(type: "bigint", nullable: false),
                    condition_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    source_field = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    @operator = table.Column<string>(name: "operator", type: "character varying(30)", maxLength: 30, nullable: false),
                    value_from = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    value_to = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    values = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dim_analytical_rule_condition_pkey", x => x.analytical_rule_condition_id);
                    table.ForeignKey(
                        name: "fk_analytical_rule_condition_item",
                        column: x => x.analytical_item_id,
                        principalSchema: "dashboard_sch",
                        principalTable: "dim_analytical_item",
                        principalColumn: "analytical_item_id");
                    table.ForeignKey(
                        name: "fk_analytical_rule_condition_rule",
                        column: x => x.analytical_rule_id,
                        principalSchema: "dashboard_sch",
                        principalTable: "dim_analytical_rule",
                        principalColumn: "analytical_rule_id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_dim_administrative_unit_active",
                schema: "dashboard_sch",
                table: "dim_administrative_unit",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_dim_administrative_unit_parent",
                schema: "dashboard_sch",
                table: "dim_administrative_unit",
                column: "parent_code");

            migrationBuilder.CreateIndex(
                name: "uq_dim_administrative_unit_code_effective",
                schema: "dashboard_sch",
                table: "dim_administrative_unit",
                columns: new[] { "code", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_dim_age_range_from_to",
                schema: "dashboard_sch",
                table: "dim_age_range",
                columns: new[] { "age_from", "age_to" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_active",
                schema: "dashboard_sch",
                table: "dim_analytical",
                column: "is_active",
                filter: "is_active = TRUE");

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_code",
                schema: "dashboard_sch",
                table: "dim_analytical",
                column: "analytical_code");

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_dimension_type",
                schema: "dashboard_sch",
                table: "dim_analytical",
                column: "dimension_type");

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_effective_period",
                schema: "dashboard_sch",
                table: "dim_analytical",
                columns: new[] { "analytical_code", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "uq_dim_analytical_code_version",
                schema: "dashboard_sch",
                table: "dim_analytical",
                columns: new[] { "analytical_code", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_item_active",
                schema: "dashboard_sch",
                table: "dim_analytical_item",
                columns: new[] { "analytical_id", "is_active" },
                filter: "is_active = TRUE");

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_item_analytical",
                schema: "dashboard_sch",
                table: "dim_analytical_item",
                column: "analytical_id");

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_item_code",
                schema: "dashboard_sch",
                table: "dim_analytical_item",
                column: "item_code");

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_item_parent",
                schema: "dashboard_sch",
                table: "dim_analytical_item",
                column: "parent_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_dim_analytical_item_sort",
                schema: "dashboard_sch",
                table: "dim_analytical_item",
                columns: new[] { "analytical_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "uq_dim_analytical_item_code",
                schema: "dashboard_sch",
                table: "dim_analytical_item",
                columns: new[] { "analytical_id", "item_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_analytical_item_translation_item",
                schema: "dashboard_sch",
                table: "dim_analytical_item_translation",
                column: "analytical_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_analytical_item_translation_language",
                schema: "dashboard_sch",
                table: "dim_analytical_item_translation",
                column: "language_code");

            migrationBuilder.CreateIndex(
                name: "uq_analytical_item_translation_language",
                schema: "dashboard_sch",
                table: "dim_analytical_item_translation",
                columns: new[] { "analytical_item_id", "language_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_active",
                schema: "dashboard_sch",
                table: "dim_analytical_rule",
                columns: new[] { "analytical_id", "is_active" },
                filter: "is_active = TRUE");

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_analytical",
                schema: "dashboard_sch",
                table: "dim_analytical_rule",
                column: "analytical_id");

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_code",
                schema: "dashboard_sch",
                table: "dim_analytical_rule",
                column: "rule_code");

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_effective_period",
                schema: "dashboard_sch",
                table: "dim_analytical_rule",
                columns: new[] { "analytical_id", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "uq_analytical_rule_code_version",
                schema: "dashboard_sch",
                table: "dim_analytical_rule",
                columns: new[] { "analytical_id", "rule_code", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_condition_item",
                schema: "dashboard_sch",
                table: "dim_analytical_rule_condition",
                column: "analytical_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_condition_order",
                schema: "dashboard_sch",
                table: "dim_analytical_rule_condition",
                columns: new[] { "analytical_rule_id", "condition_order" });

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_condition_rule",
                schema: "dashboard_sch",
                table: "dim_analytical_rule_condition",
                column: "analytical_rule_id");

            migrationBuilder.CreateIndex(
                name: "ix_analytical_rule_condition_source_field",
                schema: "dashboard_sch",
                table: "dim_analytical_rule_condition",
                column: "source_field");

            migrationBuilder.CreateIndex(
                name: "ix_dim_catalog_items_active",
                schema: "dashboard_sch",
                table: "dim_catalog_items",
                columns: new[] { "catalog_name", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_dim_industry_active",
                schema: "dashboard_sch",
                table: "dim_industry",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_dim_industry_level",
                schema: "dashboard_sch",
                table: "dim_industry",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "ix_dim_industry_parent",
                schema: "dashboard_sch",
                table: "dim_industry",
                column: "parent_code");

            migrationBuilder.CreateIndex(
                name: "uq_dim_industry_code_effective",
                schema: "dashboard_sch",
                table: "dim_industry",
                columns: new[] { "code", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dim_occupation_active",
                schema: "dashboard_sch",
                table: "dim_occupation",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_dim_occupation_level",
                schema: "dashboard_sch",
                table: "dim_occupation",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "ix_dim_occupation_parent",
                schema: "dashboard_sch",
                table: "dim_occupation",
                column: "parent_code");

            migrationBuilder.CreateIndex(
                name: "uq_dim_occupation_code_effective",
                schema: "dashboard_sch",
                table: "dim_occupation",
                columns: new[] { "code", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_labour_economic_status_summary_admin",
                schema: "dashboard_sch",
                table: "labour_economic_status_summary",
                column: "administrative_unit_code");

            migrationBuilder.CreateIndex(
                name: "ix_labour_economic_status_summary_filter",
                schema: "dashboard_sch",
                table: "labour_economic_status_summary",
                columns: new[] { "administrative_unit_code", "education_level_code", "technical_level_code" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_economic_status_summary_period",
                schema: "dashboard_sch",
                table: "labour_economic_status_summary",
                column: "reference_period");

            migrationBuilder.CreateIndex(
                name: "ix_labour_economic_status_summary_period_admin",
                schema: "dashboard_sch",
                table: "labour_economic_status_summary",
                columns: new[] { "reference_period", "administrative_unit_code" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_economic_status_summary_status",
                schema: "dashboard_sch",
                table: "labour_economic_status_summary",
                column: "economic_status_code");

            migrationBuilder.CreateIndex(
                name: "uq_labour_economic_status_summary_grain",
                schema: "dashboard_sch",
                table: "labour_economic_status_summary",
                columns: new[] { "reference_period", "administrative_unit_code", "education_level_code", "technical_level_code", "economic_status_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_labour_employment_snapshot_economic",
                schema: "dashboard_sch",
                table: "labour_employment_snapshot",
                column: "economic_activities_code");

            migrationBuilder.CreateIndex(
                name: "ix_labour_employment_snapshot_employer",
                schema: "dashboard_sch",
                table: "labour_employment_snapshot",
                column: "employer_code");

            migrationBuilder.CreateIndex(
                name: "ix_labour_employment_snapshot_industry",
                schema: "dashboard_sch",
                table: "labour_employment_snapshot",
                column: "skill_industry_code");

            migrationBuilder.CreateIndex(
                name: "ix_labour_employment_snapshot_occupation",
                schema: "dashboard_sch",
                table: "labour_employment_snapshot",
                column: "occupation_code");

            migrationBuilder.CreateIndex(
                name: "ix_labour_employment_snapshot_version",
                schema: "dashboard_sch",
                table: "labour_employment_snapshot",
                column: "snapshot_version");

            migrationBuilder.CreateIndex(
                name: "ix_labour_identity_summary_age",
                schema: "dashboard_sch",
                table: "labour_identity_summary",
                column: "age_group_code");

            migrationBuilder.CreateIndex(
                name: "ix_labour_identity_summary_filter",
                schema: "dashboard_sch",
                table: "labour_identity_summary",
                columns: new[] { "administrative_unit_code", "education_level_code", "technical_level_code" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_identity_summary_occupation",
                schema: "dashboard_sch",
                table: "labour_identity_summary",
                column: "top_level_occupation_code");

            migrationBuilder.CreateIndex(
                name: "ix_labour_identity_summary_status",
                schema: "dashboard_sch",
                table: "labour_identity_summary",
                column: "economic_status_code");

            migrationBuilder.CreateIndex(
                name: "uq_labour_identity_summary_grain",
                schema: "dashboard_sch",
                table: "labour_identity_summary",
                columns: new[] { "administrative_unit_code", "education_level_code", "technical_level_code", "top_level_occupation_code", "age_group_code", "economic_status_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_labour_time_summary_filter",
                schema: "dashboard_sch",
                table: "labour_time_summary",
                columns: new[] { "administrative_unit_code", "education_level_code", "technical_level_code" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_time_summary_period",
                schema: "dashboard_sch",
                table: "labour_time_summary",
                column: "reference_period");

            migrationBuilder.CreateIndex(
                name: "ix_labour_time_summary_period_admin",
                schema: "dashboard_sch",
                table: "labour_time_summary",
                columns: new[] { "reference_period", "administrative_unit_code" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_time_summary_revised",
                schema: "dashboard_sch",
                table: "labour_time_summary",
                column: "is_revised");

            migrationBuilder.CreateIndex(
                name: "uq_labour_time_summary_grain",
                schema: "dashboard_sch",
                table: "labour_time_summary",
                columns: new[] { "reference_period", "administrative_unit_code", "education_level_code", "technical_level_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dim_administrative_unit",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_age_range",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_analytical_item_translation",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_analytical_rule_condition",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_catalog_items",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_industry",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_occupation",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "labour_economic_status_summary",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "labour_employment_snapshot",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "labour_identity_snapshot",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "labour_identity_summary",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "labour_time_summary",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_analytical_item",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_analytical_rule",
                schema: "dashboard_sch");

            migrationBuilder.DropTable(
                name: "dim_analytical",
                schema: "dashboard_sch");
        }
    }
}
