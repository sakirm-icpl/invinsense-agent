pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                // Checkout the source code from Git
                checkout scm
            }
        }

        stage('Build') {
            steps {
                // Use MSBuild to build the C# project
                bat 'dotnet build Invinsense.sln'
            }
        }

        stage('Package') {
            steps {
                // Use make command to create exe file
                bat 'make IvsAgent'
            }
        }

        stage('Create Setup') {
            steps {
                // Add steps to create your setup file
                // For example, use Inno Setup or WiX Toolset
				bat 'dotnet build Setup.sln'
            }
        }
    }

    post {
        always {
            // Clean up or perform additional steps after the build
        }
    }
}
