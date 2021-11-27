global using System;
global using System.Data;
global using System.Diagnostics;
global using System.Linq;

global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

global using Dapper;
global using Lamar;
global using Lamar.Microsoft.DependencyInjection;
global using Prometheus;
global using Swashbuckle.AspNetCore.SwaggerUI;
