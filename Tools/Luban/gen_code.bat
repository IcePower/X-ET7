
set WORKSPACE=..\..

set GEN_CLIENT=Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\Unity\Assets\Config\Excel
set OUTPUT_CODE_DIR=%WORKSPACE%\Unity\Assets\Scripts\Codes\Model\Generate
set OUTPUT_DATA_DIR=%WORKSPACE%\Config\Excel

echo ======================= Server GameConfig ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_data_dir %OUTPUT_DATA_DIR%\s ^
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory ^
 --output:exclude_tags c ^
 --gen_types data_bin ^
 -s server
 
echo ======================= Server StartConfig Localhost ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Localhost ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\Localhost ^
 --gen_types data_bin ^
 -s server
 
echo ======================= Server StartConfig Release ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Release ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\Release ^
 --gen_types data_bin ^
 -s server
  
echo ======================= Server StartConfig RouterTest ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\RouterTest ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\RouterTest ^
 --gen_types data_bin ^
 -s server
 
echo ======================= Server StartConfig Benchmark ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Benchmark ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\Benchmark ^
 --gen_types data_bin ^
 -s server
 
echo ======================= Server Code ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %OUTPUT_CODE_DIR%\Server\Config ^
 --gen_types code_cs_bin ^
 -s server
 
echo =====================================================================================================
echo ======================= ClientServer GameConfig ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs ^
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory ^
 --gen_types data_bin ^
 -s all
 
echo ======================= ClientServer StartConfig Localhost ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Localhost ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\Localhost ^
 --gen_types data_bin ^
 -s all
 
echo ======================= ClientServer StartConfig Release ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Release ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\Release ^
 --gen_types data_bin ^
 -s all
  
echo ======================= ClientServer StartConfig RouterTest ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\RouterTest ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\RouterTest ^
 --gen_types data_bin ^
 -s all
 
echo ======================= ClientServer StartConfig Benchmark ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Benchmark ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\Benchmark ^
 --gen_types data_bin ^
 -s all
 
echo ======================= ClientServer Code ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %OUTPUT_CODE_DIR%\ClientServer\Config ^
 --gen_types code_cs_bin ^
 -s all


echo =====================================================================================================
echo ======================= Client ==========================
%GEN_CLIENT% --template_search_path Template_Client -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %OUTPUT_CODE_DIR%\Client\Config ^
 --output_data_dir %OUTPUT_DATA_DIR%\c ^
 --output:exclude_tags s \
 --gen_types code_cs_bin,data_bin ^
 -s client
 
 
echo =====================================================================================================
echo ======================= Server GameConfig Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_data_dir %OUTPUT_DATA_DIR%\s ^
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory ^
 --output:exclude_tags c ^
 --gen_types data_json ^
 -s server
 
echo ======================= Server StartConfig Localhost Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Localhost ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\Localhost ^
 --gen_types data_json ^
 -s server
 
echo ======================= Server StartConfig Release Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Release ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\Release ^
 --gen_types data_json ^
 -s server
  
echo ======================= Server StartConfig RouterTest Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\RouterTest ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\RouterTest ^
 --gen_types data_json ^
 -s server
 
echo ======================= Server StartConfig Benchmark Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Benchmark ^
 --output_data_dir %OUTPUT_DATA_DIR%\s\StartConfig\Benchmark ^
 --gen_types data_json ^
 -s server
 
 
echo =====================================================================================================
echo ======================= ClientServer GameConfig Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs ^
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory ^
 --gen_types data_json ^
 -s all
 
echo ======================= ClientServer StartConfig Localhost Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Localhost ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\Localhost ^
 --gen_types data_json ^
 -s all
 
echo ======================= ClientServer StartConfig Release Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Release ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\Release ^
 --gen_types data_json ^
 -s all
  
echo ======================= ClientServer StartConfig RouterTest Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\RouterTest ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\RouterTest ^
 --gen_types data_json ^
 -s all
 
echo ======================= ClientServer StartConfig Benchmark Json ==========================
%GEN_CLIENT% --template_search_path Template_Server -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas\StartConfig\Benchmark ^
 --output_data_dir %OUTPUT_DATA_DIR%\cs\StartConfig\Benchmark ^
 --gen_types data_json ^
 -s all
 

echo =====================================================================================================
echo ======================= Client Json ==========================
%GEN_CLIENT% --template_search_path Template_Client -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %OUTPUT_CODE_DIR%\Client\Config ^
 --output_data_dir %OUTPUT_DATA_DIR%\c ^
 --output:exclude_tags s \
 --gen_types data_json ^
 -s client
