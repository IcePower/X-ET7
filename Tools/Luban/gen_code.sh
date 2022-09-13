#!/bin/zsh
WORKSPACE=../..

GEN_CLIENT=Luban.ClientServer/Luban.ClientServer.dll
CONF_ROOT=${WORKSPACE}/Unity/Assets/Config/Excel
OUTPUT_CODE_DIR=${WORKSPACE}/Unity/Assets/Scripts/Codes/Model/Generate

echo ======================= StartConfig Localhost ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Localhost \
 --output_data_dir ${WORKSPACE}/Config/StartConfig/Localhost \
 --gen_types data_bin \
 -s server
 
echo ======================= StartConfig Release ==========================
 /usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
  -d ${CONF_ROOT}/Defines/__root__.xml \
  --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Release \
  --output_data_dir ${WORKSPACE}/Config/StartConfig/Release \
  --gen_types data_bin \
  -s server
  
echo ======================= StartConfig RouterTest ==========================
 /usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
  -d ${CONF_ROOT}/Defines/__root__.xml \
  --input_data_dir ${CONF_ROOT}/Datas/StartConfig/RouterTest \
  --output_data_dir ${WORKSPACE}/Config/StartConfig/RouterTest \
  --gen_types data_bin \
  -s server
  
echo ======================= StartConfig Benchmark ==========================
 /usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
  -d ${CONF_ROOT}/Defines/__root__.xml \
  --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Benchmark \
  --output_data_dir ${WORKSPACE}/Config/StartConfig/Benchmark \
  --gen_types data_bin \
  -s server

echo ======================= Server GameConfig ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${WORKSPACE}/Config/GameConfig \
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory \
 --gen_types data_bin \
 -s server
 
 echo ======================= Server Code ==========================
 /usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
  -d ${CONF_ROOT}/Defines/__root__.xml \
  --input_data_dir ${CONF_ROOT}/Datas \
  --output_code_dir ${OUTPUT_CODE_DIR}/Server/Config \
  --gen_types code_cs_bin \
  -s server

echo ======================= Client ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Client -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/Client/Config \
 --output_data_dir ${WORKSPACE}/Unity/Assets/Bundles/Config/GameConfig \
 --gen_types code_cs_bin,data_bin \
 -s client
 
 echo ======================= JsonForView ==========================
 /usr/local/share/dotnet/dotnet ${GEN_CLIENT} -j cfg --\
  -d ${CONF_ROOT}/Defines/__root__.xml \
  --input_data_dir ${CONF_ROOT}/Datas \
  --output_data_dir Output_Json/Server/ \
  --gen_types data_json \
  -s server
 
 /usr/local/share/dotnet/dotnet ${GEN_CLIENT} -j cfg --\
  -d ${CONF_ROOT}/Defines/__root__.xml \
  --input_data_dir ${CONF_ROOT}/Datas \
  --output_data_dir Output_Json/Client \
  --gen_types data_json \
  -s client