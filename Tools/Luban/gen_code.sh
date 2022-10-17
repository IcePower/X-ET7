#!/bin/zsh
WORKSPACE=../..

GEN_CLIENT=Luban.ClientServer/Luban.ClientServer.dll
CONF_ROOT=${WORKSPACE}/Unity/Assets/Config/Excel
OUTPUT_CODE_DIR=${WORKSPACE}/Unity/Assets/Scripts/Codes/Model/Generate
OUTPUT_DATA_DIR=${WORKSPACE}/Config/Excel
OUTPUT_JSON_DIR=${WORKSPACE}/Config/Json

#Server
echo ======================= Server GameConfig ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${OUTPUT_DATA_DIR}/s \
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory \
 --output:exclude_tags c \
 --gen_types data_bin \
 -s server
 
echo ======================= Server StartConfig Localhost ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Localhost \
 --output_data_dir ${OUTPUT_DATA_DIR}/s/StartConfig/Localhost \
 --gen_types data_bin \
 -s server
 
echo ======================= Server StartConfig Release ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Release \
 --output_data_dir ${OUTPUT_DATA_DIR}/s/StartConfig/Release \
 --gen_types data_bin \
 -s server
  
echo ======================= Server StartConfig RouterTest ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/RouterTest \
 --output_data_dir ${OUTPUT_DATA_DIR}/s/StartConfig/RouterTest \
 --gen_types data_bin \
 -s server
  
echo ======================= Server StartConfig Benchmark ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Benchmark \
 --output_data_dir ${OUTPUT_DATA_DIR}/s/StartConfig/Benchmark \
 --gen_types data_bin \
 -s server
 
echo ======================= Server Code ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/Server/Config \
 --gen_types code_cs_bin \
 -s server
  
  
#ClientServer
echo =====================================================================================================
echo ======================= ClientServer GameConfig ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${OUTPUT_DATA_DIR}/cs \
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory \
 --gen_types data_bin \
 -s all
   
echo ======================= ClientServer StartConfig Localhost ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Localhost \
 --output_data_dir ${OUTPUT_DATA_DIR}/cs/StartConfig/Localhost \
 --gen_types data_bin \
 -s all
   
echo ======================= ClientServer StartConfig Release ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Release \
 --output_data_dir ${OUTPUT_DATA_DIR}/cs/StartConfig/Release \
 --gen_types data_bin \
 -s all
    
echo ======================= ClientServer StartConfig RouterTest ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/RouterTest \
 --output_data_dir ${OUTPUT_DATA_DIR}/cs/StartConfig/RouterTest \
 --gen_types data_bin \
 -s all
    
echo ======================= ClientServer StartConfig Benchmark ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Benchmark \
 --output_data_dir ${OUTPUT_DATA_DIR}/cs/StartConfig/Benchmark \
 --gen_types data_bin \
 -s all
   
echo ======================= ClientServer Code ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/ClientServer/Config \
 --gen_types code_cs_bin \
 -s all


#Client
echo =====================================================================================================
echo ======================= Client ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Client -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/Client/Config \
 --output_data_dir ${OUTPUT_DATA_DIR}/c \
 --output:exclude_tags s \
 --gen_types code_cs_bin,data_bin \
 -s client


#Server Json
echo =====================================================================================================
echo ======================= Server GameConfig Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${OUTPUT_JSON_DIR}/s \
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory \
 --output:exclude_tags c \
 --gen_types data_json \
 -s server
 
echo ======================= Server StartConfig Localhost Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Localhost \
 --output_data_dir ${OUTPUT_JSON_DIR}/s/StartConfig/Localhost \
 --gen_types data_json \
 -s server
 
echo ======================= Server StartConfig Release Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Release \
 --output_data_dir ${OUTPUT_JSON_DIR}/s/StartConfig/Release \
 --gen_types data_json \
 -s server
  
echo ======================= Server StartConfig RouterTest Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/RouterTest \
 --output_data_dir ${OUTPUT_JSON_DIR}/s/StartConfig/RouterTest \
 --gen_types data_json \
 -s server
  
echo ======================= Server StartConfig Benchmark Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Benchmark \
 --output_data_dir ${OUTPUT_JSON_DIR}/s/StartConfig/Benchmark \
 --gen_types data_json \
 -s server
  
#ClientServer Json
echo =====================================================================================================
echo ======================= ClientServer GameConfig Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${OUTPUT_JSON_DIR}/cs \
 --output:exclude_tables StartMachineConfigCategory,StartProcessConfigCategory,StartSceneConfigCategory,StartZoneConfigCategory \
 --gen_types data_json \
 -s all
   
echo ======================= ClientServer StartConfig Localhost Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Localhost \
 --output_data_dir ${OUTPUT_JSON_DIR}/cs/StartConfig/Localhost \
 --gen_types data_json \
 -s all
   
echo ======================= ClientServer StartConfig Release Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Release \
 --output_data_dir ${OUTPUT_JSON_DIR}/cs/StartConfig/Release \
 --gen_types data_json \
 -s all
    
echo ======================= ClientServer StartConfig RouterTest Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/RouterTest \
 --output_data_dir ${OUTPUT_JSON_DIR}/cs/StartConfig/RouterTest \
 --gen_types data_json \
 -s all
    
echo ======================= ClientServer StartConfig Benchmark Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Server -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas/StartConfig/Benchmark \
 --output_data_dir ${OUTPUT_JSON_DIR}/cs/StartConfig/Benchmark \
 --gen_types data_json \
 -s all


#Client Json
echo =====================================================================================================
echo ======================= Client Json ==========================
/usr/local/share/dotnet/dotnet ${GEN_CLIENT} --template_search_path Template_Client -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/Client/Config \
 --output_data_dir ${OUTPUT_JSON_DIR}/c \
 --output:exclude_tags s \
 --gen_types data_json \
 -s client