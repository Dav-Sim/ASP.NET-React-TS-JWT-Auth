import { Form, FormikProvider, useFormik } from "formik";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useAuth, useUpdateUserProfile } from "../../query/authQueries";
import { UserProfileToUpdateDto } from "../../query/types/auth";
import toast from "react-hot-toast";
import { StronglyTypedField } from "../../components/formik/StronglyTypedField";
import { FormikInput } from "../../components/formik/FormikInput";
import { ChangePassword } from "./components/ChangePassword";
import { ActivitiesList } from "./components/ActivitiesList";

type FormData = UserProfileToUpdateDto & {};

export function UserProfilePage() {
    usePageTitle("User Profile");
    const auth = useAuth();
    const updateUser = useUpdateUserProfile();

    const formik = useFormik<FormData>({
        initialValues: {
            firstName: auth.user?.firstName ?? '',
            lastName: auth.user?.lastName ?? '',
        },
        onSubmit: async (values) => {
            await updateUser.mutateAsync(values, {
                onSuccess: () => {
                    toast.success('Profile updated successfully');
                },
                onError: (error) => {
                    toast.error(error.message);
                }
            });
        }
    });

    if (!auth.isLoggedAndVerified || !auth.user) return null;

    return (
        <div className="col-12 col-lg-6 text-center">
            <h1>Your profile</h1>

            <div className="text-start">
                <p>
                    <strong>Email:</strong> {auth.user.email}
                </p>
                <p>
                    <strong>Registration Date:</strong> {new Date(auth.user.registrationDate).toLocaleDateString()}
                </p>
                <p>
                    <strong>Email Verified:</strong> {auth.user.emailVerified ? 'Yes' : 'No'}
                </p>
                <p>
                    <strong>Email verification Date:</strong> {
                        auth.user.emailVerificationDate &&
                        new Date(auth.user.emailVerificationDate).toLocaleDateString()
                    }
                </p>
            </div>

            <hr />
            <FormikProvider value={formik}>
                <Form>
                    <div className="mb-3">
                        <StronglyTypedField<FormData>
                            name="firstName"
                            label="First Name"
                            component={FormikInput}
                        />
                    </div>
                    <div className="mb-3">
                        <StronglyTypedField<FormData>
                            name="lastName"
                            label="Last Name"
                            component={FormikInput}
                        />
                    </div>
                    <button type="submit"
                        className="btn btn-primary w-100">
                        Update Profile
                    </button>
                </Form>
            </FormikProvider>

            <hr />
            <ChangePassword />

            <hr />
            <ActivitiesList />
        </div>
    );
}