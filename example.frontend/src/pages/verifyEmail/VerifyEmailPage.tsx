import { useNavigate, useSearchParams } from "react-router-dom";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useVerifyEmail } from "../../query/authQueries";
import { useEffect } from "react";
import toast from "react-hot-toast";
import { Loading } from "../../components/loading/Loading";


export function VerifyEmailPage() {
    usePageTitle("Verify Email Page");

    const [search] = useSearchParams();
    const email = search.get("email");
    const token = search.get("token");

    const verifyEmail = useVerifyEmail();

    const nav = useNavigate();

    useEffect(() => {
        if (email && token && !verifyEmail.isPending && verifyEmail.status === 'idle') {
            verifyEmail.mutate({
                email,
                token
            }, {
                onSuccess: () => {
                    toast.success("Email verified successfully. You can now login.");
                    nav('/');
                },
                onError: (error) => {
                    toast.error(`Invalid email or token. ${error.title ?? ""}`);
                    nav('/');
                }
            });
        }
    }, [email, nav, token, verifyEmail]);

    if (!email || !token) {
        return (
            <div className="col-12 col-lg-6">
                <h1>Invalid Request</h1>
                <p>
                    The URL is invalid. Please check the URL and try again.
                </p>
            </div>
        );
    }

    return (
        <div className="col-12 col-lg-6">
            <Loading loading={verifyEmail.isPending} transparent>
                <h1>Email Verification</h1>
                <p>
                    Verifying your email address...
                </p>
            </Loading>
        </div>
    )
}