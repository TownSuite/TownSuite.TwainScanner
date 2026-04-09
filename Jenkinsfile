
library 'ts-jenkins-shared-library@main'

pipeline {
    agent none
    options {
        copyArtifactPermission('*/TownSuite-Artifact-Publish')
        buildDiscarder(logRotator(numToKeepStr: '10'))
        timestamps()
        timeout(time: 2, unit: 'HOURS')
    }
    stages {
        stage('Start Automation Script') {
            agent { label 'starting-agent' }
            steps {
                script {
                    townsuite_automation2.start_windows()
                }
            }
        }    
        stage('Pipeline') {
            agent { label townsuite_automation2.get_windows_label() }
            stages {
                stage('Environment Setup') {
                    steps {
                        script {
                            townsuite.common_environment_configuration()
                            townsuite.checkout_scm()
                        }
                    }
                }
                stage('Build') {
                    steps {
                        pwsh '''
                        .\\build.ps1
                        '''
                    }
                }
                stage('Code Sign') {
                    when {
                        expression { return env.BRANCH_NAME.startsWith('PR-') == false }
                    }
                    steps {
                        echo 'Code Signing happening here....'
                        script {
                            townsuite.codesign "${env.WORKSPACE}", "*TownSuite*.dll;*TownSuite*.exe", false
                        }
                    }
                }
                stage('Nuget Package') {
                    steps {
                        dir('nugetspec') {
                            pwsh '''
                            .\\create_nuget.ps1
                            '''
                        }
                    }
                }
                stage('Build Zip') {
                    steps {
                        pwsh '''
                        .\\build_zip.ps1
                        '''
                    }
                }
                stage('Code Sign Detached') {
                    when {
                        expression { return env.BRANCH_NAME.startsWith('PR-') == false }
                    }
                    steps {
                        echo 'Code Signing happening here....'
                        script {
                            townsuite.codesign "${env.WORKSPACE}\\build", "*.zip", true
                        }
                    }
                }
                stage('Archive') {
                    steps {
                        echo 'archiving artifacts'
                        script {
                            townsuite.archiveWithRetryAndLock('build/*.zip,build/*.SHA256SUMS,build/*.sig,build/*.nupkg', 3)
                        }
                    }
                }
            }
        }
    }
    post {
        always {
            CleanupVirtualMachines()
        }
        success {
            echo 'Pipeline executed successfully.'
        }
        failure {
            echo 'Pipeline failed.'
        }
        aborted {
            echo 'Pipeline was aborted.'
        }
    }
}

def CleanupVirtualMachines() {
    node('stopping-agent') {
        cleanWs()
        script {
            townsuite_automation2.stop_automation()
        }
    }
}
