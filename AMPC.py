import arcpy

# Set maximum number of directories to process
max_dirs = 100

# Initialize AIO object
aio = arcpy.AIO("C:/AMPC_Resources/ACS_Files/esrims_pc_sentinel-3-sral-wat-l2-netcdf.acs")

# Change to target directory
aio.chdir('/vsiaz/sentinel-3/SRAL/SR_2_WAT___/2025/01/01/')

# Get list of subdirectories and limit to max_dirs
dirs = aio.listdir()[:max_dirs]

# Process each directory
for i, dir_name in enumerate(dirs):
    aio.chdir(dir_name)
    nc_files = [f for f in aio.listdir() if f.endswith('enhanced_measurement.nc')]
    if nc_files:
        destination = f"C:\\Users\\Administrator\\Desktop\\Trajectory2\\{i}_enhanced_measurement.nc"
        aio.copy(nc_files[0], destination)
