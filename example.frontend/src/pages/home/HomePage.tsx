import { Link } from "react-router-dom";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useAuth } from "../../query/authQueries";
import { useMemo } from "react";

export function HomePage() {
    usePageTitle("Home Page");

    const auth = useAuth();

    const name = useMemo(() => {
        if (auth.user) {
            if (auth.user.firstName && auth.user.lastName) {
                return `${auth.user.firstName} ${auth.user.lastName}`;
            }
            else if (auth.user.firstName) {
                return auth.user.firstName;
            }
            else if (auth.user.lastName) {
                return auth.user.lastName;
            }
            else {
                return auth.user?.email;
            }
        }
    }, [auth.user]);


    return (
        <div className="col-12 col-lg-6 text-center">
            <h1>
                Welcome {name}
            </h1>

            <p>
                This is the home page
            </p>

            {
                auth.isLoggedAndVerified && auth.user?.isAdmin === true &&
                <div>
                    <p>
                        You are an admin
                    </p>
                    <Link to={`/admin/users`} className="btn btn-primary w-100">
                        Manage users
                    </Link>
                </div>
            }
        </div>
    );
}