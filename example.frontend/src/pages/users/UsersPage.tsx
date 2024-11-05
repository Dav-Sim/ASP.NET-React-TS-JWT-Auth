import toast from "react-hot-toast";
import { Loading } from "../../components/loading/Loading";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useDeleteUser, useUsers } from "../../query/adminQueries";
import { Question } from "../../helpers/dialogs";
import { useCallback, useMemo, useState } from "react";
import { MaterialReactTable, MRT_ColumnDef, MRT_ColumnFiltersState, MRT_ToggleFiltersButton } from "material-react-table";
import { UserDto } from "../../query/types/admin";
import { IconButton, Tooltip } from "@mui/material";
import { UserEditModal } from "./components/UserEditModal";

export function UsersPage() {
    usePageTitle("Users");

    const [globalFilter, setGlobalFilter] = useState("");
    const [filters, setFilters] = useState<MRT_ColumnFiltersState>([]);
    const [selectedUser, setSelectedUser] = useState<UserDto | undefined>(undefined);

    const users = useUsers();
    const deleteUser = useDeleteUser();

    const handleDelete = useCallback((user: UserDto) => {
        Question("Delete user",
            `Are you sure you want to delete ${user.email}?`, () => {
                deleteUser.mutate(user.id, {
                    onSuccess: () => {
                        toast.success("User deleted successfully");
                    },
                    onError: (error) => {
                        toast.error(`Failed to delete user: ${error}`);
                    }
                });
            });
    }, [deleteUser]);

    const handleEdit = useCallback((user: UserDto) => {
        setSelectedUser(user);
    }, []);

    const columns = useColumns(handleDelete, handleEdit);

    const handleClearFilters = (ev: React.SyntheticEvent<HTMLButtonElement>) => {
        ev.preventDefault();
        setFilters?.([]);
        setGlobalFilter("");
    }

    return (
        <>
            <UserEditModal user={selectedUser}
                visible={selectedUser !== undefined}
                onClose={() => setSelectedUser(undefined)}
            />

            <Loading loading={users.isLoading} className="w-100">
                <h1>Users</h1>
                <div>
                    <div style={{ overflowX: 'scroll' }}>
                        <MaterialReactTable
                            columns={columns}
                            data={users.data ?? []}
                            enableRowPinning={false}
                            enableColumnPinning={false}
                            enablePagination={true}
                            enableColumnOrdering={false}
                            enableGlobalFilter={true}
                            enableHiding={false}
                            enableStickyHeader={true}
                            sortDescFirst={true}
                            enableFilters={true}
                            enableColumnFilters={true}
                            enableDensityToggle={false}
                            enableColumnDragging={false}
                            enableColumnResizing={false}
                            enableColumnActions={false}
                            enableFullScreenToggle={false}
                            enableFilterMatchHighlighting={false}

                            state={{
                                density: 'compact',
                                columnFilters: filters,
                                globalFilter: globalFilter,
                            }}
                            initialState={{
                                pagination: {
                                    pageIndex: 0,
                                    pageSize: 100
                                },
                                showGlobalFilter: true
                            }}
                            onColumnFiltersChange={setFilters}
                            onGlobalFilterChange={setGlobalFilter}
                            manualPagination={false}
                            manualFiltering={false}
                            manualSorting={false}

                            muiTableProps={{
                                size: "small",
                            }}

                            renderToolbarInternalActions={({ table }) => (
                                <>
                                    <MRT_ToggleFiltersButton table={table} />
                                    <Tooltip arrow title="Clear all filters">
                                        <IconButton onClick={handleClearFilters}>
                                            <i className="fa-solid fa-xmark"></i>
                                        </IconButton>
                                    </Tooltip>
                                </>
                            )}
                        />
                    </div>
                </div>
            </Loading>
        </>
    );
}

function useColumns(
    handleDelete: (user: UserDto) => void,
    handleEdit: (user: UserDto) => void) {
    return useMemo<MRT_ColumnDef<UserDto>[]>(() => [
        {
            header: "Email",
            accessorFn: (row) => row.email,
        },
        {
            header: "First Name",
            accessorFn: (row) => row.firstName,
        },
        {
            header: "Last Name",
            accessorFn: (row) => row.lastName,
        },
        {
            header: "Verified",
            accessorFn: (row) => row.emailVerified ? "Yes" : "No",
        },
        {
            header: "Actions",
            accessorFn: (row) => row.id,
            enableSorting: false,
            enableFiltering: false,
            Cell: ({ row }) => (
                <div>
                    <a href="/"
                        onClick={(ev) => {
                            ev.preventDefault();
                            handleDelete(row.original);
                        }}
                        className="text-danger"
                    >
                        <i className="fa-solid fa-trash"></i>
                    </a>

                    <a href="/"
                        onClick={(ev) => {
                            ev.preventDefault();
                            handleEdit(row.original);
                        }}
                        className="text-primary ms-4"
                    >
                        <i className="fa-solid fa-pencil"></i>
                    </a>
                </div>
            )
        }
    ], [handleDelete, handleEdit]);
}