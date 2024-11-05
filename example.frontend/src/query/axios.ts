import axios from 'axios';

const api = axios.create({
    baseURL: "/api",
    validateStatus: (status) => status >= 200 && status < 300,
});

/* we are using both access token and refresh token in cookies */
api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        // Retry the request with a new token if the token has expired (signaled by a 401 response)
        if (error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                const response = await axios.post('/api/auth/refresh-token', undefined, {
                    validateStatus: (status) => status >= 200 && status < 500,
                });

                if (response.status !== 200) {
                    console.log('Refresh token request failed');
                }

                return axios(originalRequest);
            } catch (error) {
                console.error(error);
                // TODO Handle refresh token error (server error)
            }
        }

        return Promise.reject(error);
    }
);

export default api;