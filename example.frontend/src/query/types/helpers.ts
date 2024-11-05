
export const queryKeys = {
    fetch_userProfile: 'fetch_userProfile',
    fetch_userActivities: 'fetch_userActivitis',
    fetch_users: 'fetch_users',
}

export class CustomError extends Error {

    public detail: string | undefined;
    public title: string | undefined;

    constructor(message: string, detail?: string) {
        super(message);

        Object.setPrototypeOf(this, CustomError.prototype);

        this.detail = detail;
        this.title = message;
    }
}

export interface ProblemDetails {
    title?: string,
    detail?: string
}