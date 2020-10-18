using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompanyAsync(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            //if (!ModelState.IsValid)
            //{
            //    _logger.LogError("Invalid model state for the patch document");
            //    return UnprocessableEntity(ModelState);
            //}

            var employeeEntity = _mapper.Map<Employee>(employee);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesAsync(Guid companyId)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in database.");
                return NotFound();
            }

            var employees = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges: false);

            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeAsync(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"company {companyId} doesn't exist");
                return NotFound();
            }
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            return Ok(_mapper.Map<EmployeeDto>(employee));
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteEmployeeForCompanyAsync(Guid companyId, Guid id)
        {
            var employeeForCompany = HttpContext.Items["employee"] as Employee;
            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            var employeeEntity = HttpContext.Items["employee"] as Employee;
            _mapper.Map(employee, employeeEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompanyAsync(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            var employeeEntity = HttpContext.Items["employee"] as Employee;
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            patchDoc.ApplyTo(employeeToPatch, ModelState);
            TryValidateModel(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
            return NoContent();
        }
    }
}