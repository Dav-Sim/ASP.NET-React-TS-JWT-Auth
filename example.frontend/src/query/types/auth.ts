

export interface AuthenticateRequestDto {
    email: string;
    password: string;
}

export interface ForgotPasswordRequestDto {
    email: string;
}

export interface ChangePasswordRequestDto {
    password: string;
    newPassword: string;
}

export interface RegisterRequestDto {
    email: string;
    password: string;
    firstName?: string;
    lastName?: string;
}

export interface ResendEmailVerificationRequestDto {
    email: string;
}

export interface ResetPasswordRequestDto {
    email: string;
    token: string;
    password: string;
}

export interface UserProfileDto {
    email: string;
    emailVerified: boolean;
    firstName?: string;
    lastName?: string;
    registrationDate: string;
    emailVerificationDate?: string;
    isAdmin: boolean;
}

export interface ActivityDto {
    activity: string;
    activityDescription: string;
    date: string;
}

export interface UserProfileToUpdateDto {
    firstName?: string;
    lastName?: string;
}

export interface VerifyEmailRequestDto {
    email: string;
    token: string;
}