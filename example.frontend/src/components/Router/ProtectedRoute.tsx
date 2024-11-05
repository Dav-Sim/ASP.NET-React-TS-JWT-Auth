import { useNavigate } from "react-router-dom";
import { useLayoutEffect } from "react";
import { useAuth } from "../../query/authQueries";
import { Loading } from "../loading/Loading";

export function ProtectedRoute({
    children
}: Readonly<{
    children: React.ReactNode;
}>) {
    const auth = useAuth();
    const nav = useNavigate();

    useLayoutEffect(() => {
        if (!auth.isLoading && !auth.isLoggedAndVerified) {
            nav('/notauthorized');
        }
    }, [auth.isLoading, auth.isLoggedAndVerified, nav]);

    if (auth.isLoading) {
        return (
            <Loading loading={auth.isLoading}
                transparent
                style={{
                    minHeight: '100vh',
                    minWidth: '100vw'
                }}></Loading>
        );
    }
    else if (auth.isLoggedAndVerified) {
        return <>{children}</>;
    }
    else {
        return null;
    }
}