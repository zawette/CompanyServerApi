using AutoMapper;
using Contracts;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            this._logger = logger;
            this._mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetEmployees(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in database.");
                return NotFound();
            }

            var employees = _repository.Employee.GetEmployees(companyId, trackChanges: false);

            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"company {id} doesn't exist");
                return NotFound();
            }
            var employee = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            return Ok(_mapper.Map<EmployeeDto>(employee));
        }
    }
}