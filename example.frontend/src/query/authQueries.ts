import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { CustomError, ProblemDetails, queryKeys } from "./types/helpers";
import { ActivityDto, AuthenticateRequestDto, ChangePasswordRequestDto, ForgotPasswordRequestDto, RegisterRequestDto, ResendEmailVerificationRequestDto, ResetPasswordRequestDto, UserProfileDto, UserProfileToUpdateDto, VerifyEmailRequestDto } from "./types/auth";
import api from "./axios";
import axios, { AxiosError } from "axios";
import { useMemo } from "react";

const useUserProfile = () => {
    const queryClient = useQueryClient();

    return useQuery<UserProfileDto, CustomError>({
        queryKey: [queryKeys.fetch_userProfile],
        queryFn: async () => {
            const url = `auth/profile`;

            try {
                const result = await api.get<UserProfileDto>(url);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userActivities] });

                return result.data;
            }
            catch (error) {
                let message = 'fetch user error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        },
        staleTime: 1000 * 60 * 60 * 24, // 24 hours
        gcTime: 1000 * 60 * 60 * 24, // 24 hours
    })
}

export const useActivities = () => {
    return useQuery<ActivityDto[], CustomError>({
        queryKey: [queryKeys.fetch_userActivities],
        queryFn: async () => {
            const url = `auth/my-activities`;

            try {
                const result = await api.get<ActivityDto[]>(url);
                return result.data;
            }
            catch (error) {
                let message = 'fetch user activities error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        },
        staleTime: 0,
        gcTime: 0
    })
}

export const useAuth = () => {
    const user = useUserProfile();

    const data = useMemo(() => ({
        isLoading: user.isLoading,
        isError: user.isError,
        error: user.error,
        user: user.isSuccess === true ? user.data : undefined,
        isLoggedAndVerified: user.isSuccess === true && user.data.emailVerified === true,
        isLoggedButUnverified: user.isSuccess === true && user.data.emailVerified !== true,
    }), [user]);

    return { ...data };
}

export const useUpdateUserProfile = () => {
    const queryClient = useQueryClient();

    return useMutation<void, CustomError, UserProfileToUpdateDto>({
        mutationFn: async (data: UserProfileToUpdateDto) => {
            const url = `auth/profile`;

            try {
                await api.put(url, data);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userProfile] });
                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userActivities] });

                return;
            }
            catch (error) {
                let message = 'update user profile error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useLogin = () => {
    const queryClient = useQueryClient();

    return useMutation<UserProfileDto, CustomError, AuthenticateRequestDto>({
        mutationFn: async (data: AuthenticateRequestDto) => {
            const url = `auth/login`;

            try {
                const result = await api.post<UserProfileDto>(url, data);

                await queryClient.setQueryData([queryKeys.fetch_userProfile], result.data);
                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userActivities] });

                return result.data;
            }
            catch (error) {
                let message = 'login error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useLogout = () => {
    const queryClient = useQueryClient();

    return useMutation<void, CustomError>({
        mutationFn: async () => {
            const url = `auth/revoke-token`;

            try {
                await api.post(url);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userProfile] });

                return;
            }
            catch (error) {
                let message = 'logout error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userProfile] });

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useRegister = () => {
    const queryClient = useQueryClient();

    return useMutation<UserProfileDto, CustomError, RegisterRequestDto>({
        mutationFn: async (data: RegisterRequestDto) => {
            const url = `auth/register`;

            try {
                const result = await api.post<UserProfileDto>(url, data);

                await queryClient.setQueryData([queryKeys.fetch_userProfile], result.data);
                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userActivities] });

                return result.data;
            }
            catch (error) {
                let message = 'register error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useVerifyEmail = () => {
    const queryClient = useQueryClient();

    return useMutation<void, CustomError, VerifyEmailRequestDto>({
        mutationFn: async (data: VerifyEmailRequestDto) => {
            const url = `auth/verify-email`;

            try {
                await api.post(url, data);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userProfile] });
                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userActivities] });

                return;
            }
            catch (error) {
                let message = 'verify email error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useResendEmailVerification = () => {
    return useMutation<void, CustomError, ResendEmailVerificationRequestDto>({
        mutationFn: async (data: ResendEmailVerificationRequestDto) => {
            const url = `auth/resend-email-verification`;

            try {
                await api.post(url, data);

                return;
            }
            catch (error) {
                let message = 'resend email verification error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useForgotPassword = () => {
    return useMutation<void, CustomError, ForgotPasswordRequestDto>({
        mutationFn: async (data: { email: string }) => {
            const url = `auth/forgot-password`;

            try {
                await api.post(url, data);

                return;
            }
            catch (error) {
                let message = 'forgot password error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useResetPassword = () => {
    const queryClient = useQueryClient();

    return useMutation<void, CustomError, ResetPasswordRequestDto>({
        mutationFn: async (data: ResetPasswordRequestDto) => {
            const url = `auth/reset-forgotten-password`;

            try {
                await api.post(url, data);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userProfile] });
                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userActivities] });

                return;
            }
            catch (error) {
                let message = 'reset password error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useChangePassword = () => {
    const queryClient = useQueryClient();

    return useMutation<void, CustomError, ChangePasswordRequestDto>({
        mutationFn: async (data: ChangePasswordRequestDto) => {
            const url = `auth/change-password`;

            try {
                await api.post(url, data);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userProfile] });
                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_userActivities] });

                return;
            }
            catch (error) {
                let message = 'change password error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}