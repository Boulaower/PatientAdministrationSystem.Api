using Microsoft.AspNetCore.Mvc;
using PatientAdministrationSystem.Application.Interfaces;
using PatientAdministrationSystem.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Hci.Ah.Home.Api.Gateway.Controllers.Patients
{
    [Route("api/patients")]
    [ApiExplorerSettings(GroupName = "Patients")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientsService _patientsService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientsService patientsService, ILogger<PatientsController> logger)
        {
            _patientsService = patientsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            try
            {
                var patients = await _patientsService.GetAllPatientsAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all patients.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchPatients([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name cannot be empty.");

            try
            {
                var patients = await _patientsService.SearchPatientsByNameAsync(name);
                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for patients with name {Name}.", name);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById([FromRoute] Guid id) // Use [FromRoute] attribute
        {
            try
            {
                var patient = await _patientsService.GetPatientByIdAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found.");

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the patient with ID {Id}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] PatientDto patientDto)
        {
            if (patientDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdPatient = await _patientsService.CreatePatientAsync(patientDto);
                return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new patient.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient([FromRoute] Guid id, [FromBody] PatientDto patientDto) // Use [FromRoute] attribute
        {
            if (patientDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _patientsService.UpdatePatientAsync(id, patientDto);
                if (!updated)
                    return NotFound($"Patient with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the patient with ID {Id}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient([FromRoute] Guid id) // Use [FromRoute] attribute
        {
            try
            {
                var deleted = await _patientsService.DeletePatientAsync(id);
                if (!deleted)
                    return NotFound($"Patient with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the patient with ID {Id}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
