using AutoMapper;
using Blogging.Api.Extensions;
using Blogging.Api.Mapper;
using Blogging.Api.Services;
using Blogging.Api.Validations;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseService();

builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddValidatorsFromAssemblyContaining<PostValidator>();

builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseSwagger();

app.UseSwaggerUI();

app.ApplyMigrationAndSeedData();

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
