-- Enable IDENTITY_INSERT for Status table to insert specific status_id values
SET IDENTITY_INSERT Status ON;

-- Insert sample data for Status table if not exists with specific IDs
IF NOT EXISTS (SELECT 1 FROM Status WHERE status_id = 0)
INSERT INTO Status (status_id, status_name, status_type) VALUES (0, 'Pending', 'Account');

IF NOT EXISTS (SELECT 1 FROM Status WHERE status_id = 1)
INSERT INTO Status (status_id, status_name, status_type) VALUES (1, 'Active', 'Account');

IF NOT EXISTS (SELECT 1 FROM Status WHERE status_id = 2)
INSERT INTO Status (status_id, status_name, status_type) VALUES (2, 'Inactive', 'Account');

IF NOT EXISTS (SELECT 1 FROM Status WHERE status_id = 3)
INSERT INTO Status (status_id, status_name, status_type) VALUES (3, 'Blocked', 'Account');

-- Disable IDENTITY_INSERT for Status table
SET IDENTITY_INSERT Status OFF;

-- Enable IDENTITY_INSERT for Role table to insert specific role_id values
SET IDENTITY_INSERT Role ON;

-- Insert sample data for Role table if not exists
IF NOT EXISTS (SELECT 1 FROM Role WHERE role_id = 1)
INSERT INTO Role (role_id, role_name) VALUES (1, 'Admin');

IF NOT EXISTS (SELECT 1 FROM Role WHERE role_id = 2)
INSERT INTO Role (role_id, role_name) VALUES (2, 'Manager');

IF NOT EXISTS (SELECT 1 FROM Role WHERE role_id = 3)
INSERT INTO Role (role_id, role_name) VALUES (3, 'Customer');

IF NOT EXISTS (SELECT 1 FROM Role WHERE role_id = 4)
INSERT INTO Role (role_id, role_name) VALUES (4, 'Supporter');

IF NOT EXISTS (SELECT 1 FROM Role WHERE role_id = 5)
INSERT INTO Role (role_id, role_name) VALUES (5, 'Guest');

-- Disable IDENTITY_INSERT for Role table
SET IDENTITY_INSERT Role OFF;

-- Insert Permission Pages for Registration
IF NOT EXISTS (SELECT 1 FROM PermissionPage WHERE url = '/Register')
INSERT INTO PermissionPage (name, url) VALUES ('Register Page', '/Register');

IF NOT EXISTS (SELECT 1 FROM PermissionPage WHERE url = '/Login/Register')
INSERT INTO PermissionPage (name, url) VALUES ('Register Page Alt', '/Login/Register');

-- Insert Permission APIs for Registration
IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/Register')
INSERT INTO PermissionAPI (name, url) VALUES ('Register API', '/api/Register');

IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/Register/check-username')
INSERT INTO PermissionAPI (name, url) VALUES ('Check Username API', '/api/Register/check-username');

IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/Register/check-email')
INSERT INTO PermissionAPI (name, url) VALUES ('Check Email API', '/api/Register/check-email');

-- Insert Permission Pages for Forgot Password
IF NOT EXISTS (SELECT 1 FROM PermissionPage WHERE url = '/ForgotPassword')
INSERT INTO PermissionPage (name, url) VALUES ('Forgot Password Page', '/ForgotPassword');

IF NOT EXISTS (SELECT 1 FROM PermissionPage WHERE url = '/ForgotPassword/Reset')
INSERT INTO PermissionPage (name, url) VALUES ('Reset Password Page', '/ForgotPassword/Reset');

-- Insert Permission APIs for Forgot Password
IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/ForgotPassword/send-email')
INSERT INTO PermissionAPI (name, url) VALUES ('Send Forgot Password Email API', '/api/ForgotPassword/send-email');

IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/ForgotPassword/reset-password')
INSERT INTO PermissionAPI (name, url) VALUES ('Reset Password API', '/api/ForgotPassword/reset-password');

IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/ForgotPassword/validate-token')
INSERT INTO PermissionAPI (name, url) VALUES ('Validate Reset Token API', '/api/ForgotPassword/validate-token');

-- Insert Permission Pages for Email Verification
IF NOT EXISTS (SELECT 1 FROM PermissionPage WHERE url = '/Register/VerifyEmail')
INSERT INTO PermissionPage (name, url) VALUES ('Email Verification Page', '/Register/VerifyEmail');

IF NOT EXISTS (SELECT 1 FROM PermissionPage WHERE url = '/Register/Success')
INSERT INTO PermissionPage (name, url) VALUES ('Registration Success Page', '/Register/Success');

-- Insert Permission APIs for Email Verification
IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/EmailVerification/verify')
INSERT INTO PermissionAPI (name, url) VALUES ('Verify Email API', '/api/EmailVerification/verify');

IF NOT EXISTS (SELECT 1 FROM PermissionAPI WHERE url = '/api/EmailVerification/validate-token')
INSERT INTO PermissionAPI (name, url) VALUES ('Validate Email Verification Token API', '/api/EmailVerification/validate-token');

-- Grant Guest role access to Registration pages and APIs
DECLARE @GuestRoleId INT = (SELECT role_id FROM Role WHERE role_name = 'Guest');
DECLARE @RegisterPageId INT = (SELECT id FROM PermissionPage WHERE url = '/Register');
DECLARE @RegisterPageAltId INT = (SELECT id FROM PermissionPage WHERE url = '/Login/Register');
DECLARE @RegisterApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/Register');
DECLARE @CheckUsernameApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/Register/check-username');
DECLARE @CheckEmailApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/Register/check-email');

-- Declare variables for Forgot Password permissions
DECLARE @ForgotPasswordPageId INT = (SELECT id FROM PermissionPage WHERE url = '/ForgotPassword');
DECLARE @ResetPasswordPageId INT = (SELECT id FROM PermissionPage WHERE url = '/ForgotPassword/Reset');
DECLARE @SendEmailApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/ForgotPassword/send-email');
DECLARE @ResetPasswordApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/ForgotPassword/reset-password');
DECLARE @ValidateTokenApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/ForgotPassword/validate-token');

-- Declare variables for Email Verification permissions
DECLARE @VerifyEmailPageId INT = (SELECT id FROM PermissionPage WHERE url = '/Register/VerifyEmail');
DECLARE @RegisterSuccessPageId INT = (SELECT id FROM PermissionPage WHERE url = '/Register/Success');
DECLARE @VerifyEmailApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/EmailVerification/verify');
DECLARE @ValidateVerifyTokenApiId INT = (SELECT id FROM PermissionAPI WHERE url = '/api/EmailVerification/validate-token');

-- Link Guest role with Registration permissions
IF NOT EXISTS (SELECT 1 FROM RolePermissionPage WHERE role_id = @GuestRoleId AND permission_page_id = @RegisterPageId)
INSERT INTO RolePermissionPage (role_id, permission_page_id) VALUES (@GuestRoleId, @RegisterPageId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionPage WHERE role_id = @GuestRoleId AND permission_page_id = @RegisterPageAltId)
INSERT INTO RolePermissionPage (role_id, permission_page_id) VALUES (@GuestRoleId, @RegisterPageAltId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @RegisterApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @RegisterApiId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @CheckUsernameApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @CheckUsernameApiId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @CheckEmailApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @CheckEmailApiId);

-- Link Guest role with Forgot Password permissions
IF NOT EXISTS (SELECT 1 FROM RolePermissionPage WHERE role_id = @GuestRoleId AND permission_page_id = @ForgotPasswordPageId)
INSERT INTO RolePermissionPage (role_id, permission_page_id) VALUES (@GuestRoleId, @ForgotPasswordPageId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionPage WHERE role_id = @GuestRoleId AND permission_page_id = @ResetPasswordPageId)
INSERT INTO RolePermissionPage (role_id, permission_page_id) VALUES (@GuestRoleId, @ResetPasswordPageId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @SendEmailApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @SendEmailApiId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @ResetPasswordApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @ResetPasswordApiId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @ValidateTokenApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @ValidateTokenApiId);

-- Link Guest role with Email Verification permissions
IF NOT EXISTS (SELECT 1 FROM RolePermissionPage WHERE role_id = @GuestRoleId AND permission_page_id = @VerifyEmailPageId)
INSERT INTO RolePermissionPage (role_id, permission_page_id) VALUES (@GuestRoleId, @VerifyEmailPageId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionPage WHERE role_id = @GuestRoleId AND permission_page_id = @RegisterSuccessPageId)
INSERT INTO RolePermissionPage (role_id, permission_page_id) VALUES (@GuestRoleId, @RegisterSuccessPageId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @VerifyEmailApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @VerifyEmailApiId);

IF NOT EXISTS (SELECT 1 FROM RolePermissionAPI WHERE role_id = @GuestRoleId AND permission_api_id = @ValidateVerifyTokenApiId)
INSERT INTO RolePermissionAPI (role_id, permission_api_id) VALUES (@GuestRoleId, @ValidateVerifyTokenApiId);

-- You can check the data by running:
-- SELECT * FROM Role;
-- SELECT * FROM Status;
-- SELECT * FROM PermissionPage;
-- SELECT * FROM PermissionAPI;
-- SELECT * FROM RolePermissionPage WHERE role_id = (SELECT role_id FROM Role WHERE role_name = 'Guest');
-- SELECT * FROM RolePermissionAPI WHERE role_id = (SELECT role_id FROM Role WHERE role_name = 'Guest');
