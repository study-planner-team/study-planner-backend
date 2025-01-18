﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudyPlannerAPI.Data;

#nullable disable

namespace StudyPlannerAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241223170425_AddQuizAssignmentModel")]
    partial class AddQuizAssignmentModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuestionId"));

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.HasKey("QuestionId");

                    b.HasIndex("QuizId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.QuestionOption", b =>
                {
                    b.Property<int>("OptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OptionId"));

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<string>("OptionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("OptionId");

                    b.HasIndex("QuestionId");

                    b.ToTable("QuestionOptions");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.Quiz", b =>
                {
                    b.Property<int>("QuizId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuizId"));

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StudyPlanId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QuizId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("StudyPlanId");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.QuizAssignment", b =>
                {
                    b.Property<int>("AssignmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssignmentId"));

                    b.Property<DateTime>("AssignedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("AssignedToUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CompletedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.Property<double?>("Score")
                        .HasColumnType("float");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("AssignmentId");

                    b.HasIndex("AssignedToUserId");

                    b.HasIndex("QuizId");

                    b.ToTable("QuizAssignments");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyMaterials.StudyMaterial", b =>
                {
                    b.Property<int>("StudyMaterialId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudyMaterialId"));

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StudyTopicId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StudyMaterialId");

                    b.HasIndex("StudyTopicId");

                    b.ToTable("StudyMaterials");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyPlans.StudyPlan", b =>
                {
                    b.Property<int>("StudyPlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudyPlanId"));

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("StudyPlanId");

                    b.HasIndex("UserId");

                    b.ToTable("StudyPlans");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyPlans.StudyPlanMembers", b =>
                {
                    b.Property<int>("MembershipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MembershipId"));

                    b.Property<DateTime>("JoinedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudyPlanId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("MembershipId");

                    b.HasIndex("StudyPlanId");

                    b.HasIndex("UserId");

                    b.ToTable("StudyPlanMembers");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudySessions.StudySession", b =>
                {
                    b.Property<int>("StudySessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudySessionId"));

                    b.Property<TimeSpan?>("ActualDuration")
                        .HasColumnType("time");

                    b.Property<TimeSpan?>("ActualStartTime")
                        .HasColumnType("time");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double>("Duration")
                        .HasColumnType("float");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("StudyPlanId")
                        .HasColumnType("int");

                    b.Property<int>("TopicId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("StudySessionId");

                    b.HasIndex("StudyPlanId");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("StudySessions");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyTopics.StudyTopic", b =>
                {
                    b.Property<int>("TopicId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TopicId"));

                    b.Property<double>("Hours")
                        .HasColumnType("float");

                    b.Property<int>("StudyPlanId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TopicId");

                    b.HasIndex("StudyPlanId");

                    b.ToTable("StudyTopics");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Users.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsGoogleUser")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.Question", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.Quizes.Quiz", "Quiz")
                        .WithMany("Questions")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Quiz");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.QuestionOption", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.Quizes.Question", "Question")
                        .WithMany("Options")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.Quiz", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.Users.User", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("StudyPlannerAPI.Models.StudyPlans.StudyPlan", "StudyPlan")
                        .WithMany("Quizzes")
                        .HasForeignKey("StudyPlanId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("CreatedByUser");

                    b.Navigation("StudyPlan");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.QuizAssignment", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.Users.User", "AssignedToUser")
                        .WithMany()
                        .HasForeignKey("AssignedToUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudyPlannerAPI.Models.Quizes.Quiz", "Quiz")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedToUser");

                    b.Navigation("Quiz");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyMaterials.StudyMaterial", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.StudyTopics.StudyTopic", "StudyTopic")
                        .WithMany("StudyMaterials")
                        .HasForeignKey("StudyTopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StudyTopic");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyPlans.StudyPlan", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyPlans.StudyPlanMembers", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.StudyPlans.StudyPlan", "StudyPlan")
                        .WithMany()
                        .HasForeignKey("StudyPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudyPlannerAPI.Models.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("StudyPlan");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudySessions.StudySession", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.StudyPlans.StudyPlan", "StudyPlan")
                        .WithMany("StudySessions")
                        .HasForeignKey("StudyPlanId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("StudyPlannerAPI.Models.StudyTopics.StudyTopic", "StudyTopic")
                        .WithMany()
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudyPlannerAPI.Models.Users.User", "User")
                        .WithMany("StudySessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("StudyPlan");

                    b.Navigation("StudyTopic");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyTopics.StudyTopic", b =>
                {
                    b.HasOne("StudyPlannerAPI.Models.StudyPlans.StudyPlan", "StudyPlan")
                        .WithMany("StudyTopics")
                        .HasForeignKey("StudyPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StudyPlan");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.Question", b =>
                {
                    b.Navigation("Options");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Quizes.Quiz", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyPlans.StudyPlan", b =>
                {
                    b.Navigation("Quizzes");

                    b.Navigation("StudySessions");

                    b.Navigation("StudyTopics");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.StudyTopics.StudyTopic", b =>
                {
                    b.Navigation("StudyMaterials");
                });

            modelBuilder.Entity("StudyPlannerAPI.Models.Users.User", b =>
                {
                    b.Navigation("StudySessions");
                });
#pragma warning restore 612, 618
        }
    }
}
