var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ocent_Backend_WebApi>("webapi");

builder.Build().Run();