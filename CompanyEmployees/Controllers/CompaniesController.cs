using AutoMapper;
using Contracts;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            this._logger = logger;
            this._mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = _repository.Company.GetAllCompanies(trackChanges: false);

            return Ok(_mapper.Map<IEnumerable<CompanyDto>>(companies));
        }

        [HttpGet("{id}")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"company {id} doesn't exist");
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<CompanyDto>(company));
            }
        }

        public IActionResult CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
        {
            if (companyForCreationDto == null)
            {
                _logger.LogError("Null companyForCreation object sent from client");
                return BadRequest();
            }
            var company = _mapper.Map<Company>(companyForCreationDto);
            _repository.Company.CreateCompany(company);
            _repository.Save();
            var companyToReturn = _mapper.Map<CompanyDto>(company);
            return CreatedAtAction(nameof(GetCompany), new { id = companyToReturn.Id }, companyToReturn);
        }

    }
}