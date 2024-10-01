// Import necessary namespaces for the controller
using Microsoft.AspNetCore.Mvc;
using PatientAdministrationSystem.Application.Interfaces;
using PatientAdministrationSystem.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// Define the namespace for the PatientsController
namespace Hci.Ah.Home.Api.Gateway.Controllers.Patients
{
    // Define the route for the controller and API settings
    [Route("api/patients")]
    [ApiExplorerSettings(GroupName = "Patients")] // Group the API endpoints under "Patients" in documentation (e.g., Swagger)
    [ApiController]
    public class PatientsController : ControllerBase
    {
        // Define private fields for the IPatientsService and ILogger
        private readonly IPatientsService _patientsService;
        private readonly ILogger<PatientsController> _logger;

        // Constructor to initialize the PatientsController with a service and logger
        public PatientsController(IPatientsService patientsService, ILogger<PatientsController> logger)
        {
            _patientsService = patientsService;
            _logger = logger;
        }

        // HTTP GET: Retrieve all patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            try
            {
                // Fetch all patients using the service
                var patients = await _patientsService.GetAllPatientsAsync();
                return Ok(patients); // Return a 200 OK response with the list of patients
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred while retrieving all patients.");
                return StatusCode(500, "An error occurred while processing your request."); // Return a 500 Internal Server Error response
            }
        }

        // HTTP GET: Search for patients by name (Query string parameter)
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchPatients([FromQuery] string name)
        {
            // Validate the name parameter
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name cannot be empty."); // Return a 400 Bad Request response

            try
            {
                // Search for patients with the specified name
                var patients = await _patientsService.SearchPatientsByNameAsync(name);
                return Ok(patients); // Return a 200 OK response with the search results
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred while searching for patients with name {Name}.", name);
                return StatusCode(500, "An error occurred while processing your request."); // Return a 500 Internal Server Error response
            }
        }

        // HTTP GET: Retrieve a patient by their unique ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById([FromRoute] Guid id) // Use [FromRoute] attribute
        {
            try
            {
                // Fetch the patient by their ID using the service
                var patient = await _patientsService.GetPatientByIdAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found."); // Return a 404 Not Found response if patient is not found

                return Ok(patient); // Return a 200 OK response with the patient data
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred while retrieving the patient with ID {Id}.", id);
                return StatusCode(500, "An error occurred while processing your request."); // Return a 500 Internal Server Error response
            }
        }

        // HTTP POST: Create a new patient
        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] PatientDto patientDto)
        {
            // Validate the patient DTO
            if (patientDto == null || !ModelState.IsValid)
                return BadRequest(ModelState); // Return a 400 Bad Request response if the model is invalid

            try
            {
                // Create a new patient using the service
                var createdPatient = await _patientsService.CreatePatientAsync(patientDto);
                // Return a 201 Created response with the location of the new patient resource
                return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred while creating a new patient.");
                return StatusCode(500, "An error occurred while processing your request."); // Return a 500 Internal Server Error response
            }
        }

        // HTTP PUT: Update an existing patient by their ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient([FromRoute] Guid id, [FromBody] PatientDto patientDto) // Use [FromRoute] attribute
        {
            // Validate the patient DTO
            if (patientDto == null || !ModelState.IsValid)
                return BadRequest(ModelState); // Return a 400 Bad Request response if the model is invalid

            try
            {
                // Update the patient using the service
                var updated = await _patientsService.UpdatePatientAsync(id, patientDto);
                if (!updated)
                    return NotFound($"Patient with ID {id} not found."); // Return a 404 Not Found response if patient is not found

                return NoContent(); // Return a 204 No Content response on successful update
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred while updating the patient with ID {Id}.", id);
                return StatusCode(500, "An error occurred while processing your request."); // Return a 500 Internal Server Error response
            }
        }

        // HTTP DELETE: Delete a patient by their ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient([FromRoute] Guid id) // Use [FromRoute] attribute
        {
            try
            {
                // Delete the patient using the service
                var deleted = await _patientsService.DeletePatientAsync(id);
                if (!deleted)
                    return NotFound($"Patient with ID {id} not found."); // Return a 404 Not Found response if patient is not found

                return NoContent(); // Return a 204 No Content response on successful deletion
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs
                _logger.LogError(ex, "An error occurred while deleting the patient with ID {Id}.", id);
                return StatusCode(500, "An error occurred while processing your request."); // Return a 500 Internal Server Error response
            }
        }
    }
}
