using Microsoft.AspNetCore.Mvc;
using QApplication.Interfaces;

namespace QAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController: ControllerBase
{
    private readonly IEmployeeService _service;
    
}