export interface UserDto {
    id: number;
    email: string;
    firstName?: string;
    lastName?: string;
    registrationDate: string;
    emailVerified: boolean;
    emailVerificationDate?: string;
    isAdmin: boolean;
}