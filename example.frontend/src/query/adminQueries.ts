import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { UserDto } from "./types/admin";
import { CustomError, ProblemDetails, queryKeys } from "./types/helpers";
import api from "./axios";
import axios, { AxiosError } from "axios";


export const useUsers = () => {
    return useQuery<UserDto[], CustomError>({
        queryKey: [queryKeys.fetch_users],
        queryFn: async () => {
            const url = `admin/user`;

            try {
                const result = await api.get<UserDto[]>(url);
                return result.data;
            }
            catch (error) {
                let message = 'fetch user error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useUpdateUser = () => {
    const queryClient = useQueryClient();

    return useMutation<void, CustomError, UserDto>({
        mutationFn: async (data: UserDto) => {
            const url = `admin/user/${data.id}`;

            try {
                await api.put(url, data);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_users] });

                return;
            }
            catch (error) {
                let message = 'update user error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}

export const useDeleteUser = () => {
    const queryClient = useQueryClient();

    return useMutation<void, CustomError, number>({
        mutationFn: async (id: number) => {
            const url = `admin/user/${id}`;

            try {
                await api.delete(url);

                await queryClient.invalidateQueries({ queryKey: [queryKeys.fetch_users] });

                return;
            }
            catch (error) {
                let message = 'delete user error';
                const err = error as AxiosError<ProblemDetails>;

                if (axios.isAxiosError(err) && err?.response?.data)
                    message = err.response.data?.title ?? JSON.stringify(err.response.data);

                return Promise.reject(new CustomError(message));
            }
        }
    })
}
