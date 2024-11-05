import { MaterialReactTable, MRT_ColumnDef, MRT_ColumnFiltersState, MRT_ToggleFiltersButton } from "material-react-table";
import { Loading } from "../../../components/loading/Loading";
import { useActivities } from "../../../query/authQueries";
import { useMemo, useState } from "react";
import { IconButton, Tooltip } from "@mui/material";
import { ActivityDto } from "../../../query/types/auth";

export function ActivitiesList() {
    const activities = useActivities();

    const [globalFilter, setGlobalFilter] = useState("");
    const [filters, setFilters] = useState<MRT_ColumnFiltersState>([]);

    const columns = useColumns();

    const handleClearFilters = (ev: React.SyntheticEvent<HTMLButtonElement>) => {
        ev.preventDefault();
        setFilters?.([]);
        setGlobalFilter("");
    }

    return (
        <div>
            <h3>Activities</h3>
            <Loading loading={activities.isLoading}>
                <div>
                    <div style={{ overflowX: 'scroll' }}>
                        <MaterialReactTable
                            columns={columns}
                            data={activities.data ?? []}
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
                                sorting: [{
                                    id: 'date',
                                    desc: true
                                }],
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
        </div>
    );
}

function useColumns() {
    return useMemo<MRT_ColumnDef<ActivityDto>[]>(() => [
        {
            header: "Date",
            id: "date",
            sortingFn: (a, b) => a.original.date.localeCompare(b.original.date),
            accessorFn: (row) => new Date(row.date).toLocaleString(),
        },
        {
            header: "Activity",
            accessorFn: (row) => row.activity,
        },
        {
            header: "Description",
            accessorFn: (row) => row.activityDescription,
        }
    ], []);
}