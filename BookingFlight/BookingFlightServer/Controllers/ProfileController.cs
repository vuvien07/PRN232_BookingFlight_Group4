using BookingFlightServer.DTO.Profile;
using BookingFlightServer.Services;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Get username from JWT token
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var username = JwtDecoder.GetUsernameFromToken(token);

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { 
                        success = false, 
                        message = "Invalid token or username not found" 
                    });
                }

                var profile = await _profileService.GetProfileByUsernameAsync(username);
                
                if (profile == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Profile not found" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    data = profile,
                    message = "Profile retrieved successfully" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Error retrieving profile: " + ex.Message 
                });
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="updateRequest">Profile update data</param>
        /// <returns>Updated profile information</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDTO updateRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Invalid input data",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Get username from JWT token
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var username = JwtDecoder.GetUsernameFromToken(token);

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { 
                        success = false, 
                        message = "Invalid token or username not found" 
                    });
                }

                var updatedProfile = await _profileService.UpdateProfileAsync(username, updateRequest);
                
                if (updatedProfile == null)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Failed to update profile" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    data = updatedProfile,
                    message = "Profile updated successfully" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Error updating profile: " + ex.Message 
                });
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="changePasswordRequest">Password change data</param>
        /// <returns>Success status</returns>
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Invalid input data",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Validate new password and confirm password match
                if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmPassword)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "New password and confirm password do not match" 
                    });
                }

                // Get username from JWT token
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var username = JwtDecoder.GetUsernameFromToken(token);

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { 
                        success = false, 
                        message = "Invalid token or username not found" 
                    });
                }

                var result = await _profileService.ChangePasswordAsync(
                    username, 
                    changePasswordRequest.CurrentPassword, 
                    changePasswordRequest.NewPassword
                );

                if (!result)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Failed to change password. Please check your current password." 
                    });
                }

                return Ok(new { 
                    success = true, 
                    message = "Password changed successfully" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Error changing password: " + ex.Message 
                });
            }
        }
    }
}
