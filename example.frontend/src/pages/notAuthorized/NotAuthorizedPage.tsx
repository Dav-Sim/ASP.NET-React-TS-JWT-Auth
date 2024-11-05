import { Link, useNavigate } from "react-router-dom";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useAuth, useResendEmailVerification } from "../../query/authQueries";
import { useEffect, useMemo } from "react";
import toast from "react-hot-toast";


export function NotAuthorizedPage() {
    usePageTitle("Not Authorized");
    const auth = useAuth();
    const nav = useNavigate();

    const userStatus = useMemo(() => {
        if (auth.isError) {
            return "error";
        }
        else if (auth.isLoggedButUnverified) {
            return "notVerified";
        }
        else if (auth.isLoggedAndVerified) {
            return "authorized";
        }
        else {
            return "loading";
        }
    }, [auth]);

    useEffect(() => {
        if (userStatus === "authorized") {
            nav("/");
        }
    }, [userStatus, nav]);

    switch (userStatus) {
        case "error":
            return <NotAuthorized />;
        case "notVerified":
            return <NotVerified />;
        case "authorized":
            return <NotAuthorized />;
        case "loading":
            return null;
    }
}

function NotVerified() {
    const resend = useResendEmailVerification();
    const auth = useAuth();

    return (
        <div className="col-12 col-lg-6">
            <h1>Email Verification</h1>
            <p>
                An email has been sent to your email address. Please check your email and click on the link to verify your email address.
            </p>
            <p>
                If you did not receive the email, you can click the button below to resend the email.
            </p>
            <button
                className="btn btn-primary"
                disabled={resend.isPending}
                onClick={() => {
                    resend.mutate({
                        email: auth.user?.email ?? ''
                    }, {
                        onSuccess: () => {
                            toast.success('Email sent');
                        },
                        onError: () => {
                            toast.error("Please try again after 1 minute");
                        }
                    });
                }}
            >
                <i className="fa-solid fa-arrows-rotate me-2"></i>
                Resend Email
            </button>
        </div>
    );
}

function NotAuthorized() {
    return (
        <div className="col-12 col-lg-6 text-center">
            <h1>
                Not Authorized
            </h1>
            <p>
                You are not authorized to view this page. Please login or register.
            </p>

            <div className="d-flex flex-column gap-2 align-items-stretch"
                style={{ minWidth: '50%' }}>
                <Link to="/login" className="btn btn-outline-secondary">Login</Link>
                <span>or</span>
                <Link to="/register" className="btn btn-outline-secondary">Register</Link>
            </div>
        </div>
    );
}